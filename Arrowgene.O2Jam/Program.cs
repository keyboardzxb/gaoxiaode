// Arrowgene.O2Jam/Program.cs
using System;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

// 命名空间保持您项目原样
namespace Arrowgene.O2Jam
{
    class Program
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(Program));
        private static readonly object ConsoleLock = new();
        private static readonly Setting Setting = new();

        // 【第一处修改】在这里声明我们的游戏逻辑HTTP服务器
        private static HttpServer _gameHttpServer;

        static void Main(string[] args)
        {
            // --- 日志和数据库部分，完全保持您原有的逻辑 ---
            LogProvider.Configure<ServerLogger>(Setting);
            LogProvider.OnLogWrite += LogProviderOnOnLogWrite;
            LogProvider.Start();

            // 【第二处修改】在这里启动我们专门用于处理游戏内请求的HTTP服务器
            try
            {
                // 注意：http://+:80/ 需要以管理员身份运行程序。
                // 如果您的HttpRegistrationServer也使用了80端口，这里会冲突。
                // 您需要确保两个服务器使用不同的端口。游戏客户端请求的通常是80端口。
                _gameHttpServer = new HttpServer("http://+:80/");
                _gameHttpServer.Start();
            }
            catch (Exception ex)
            {
                Logger.Error($"FATAL: Game Logic HTTP Server start failed: {ex.Message}");
                Logger.Error("Please ensure you are running as an Administrator and port 80 is not already in use by another service (like HttpRegistrationServer or IIS).");
                Console.ReadKey();
                return;
            }


            if (args.Length == 0)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var optionsBuilder = new DbContextOptionsBuilder<O2JamDbContext>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                // Programmatically ensure TrustServerCertificate is true to fix SSL connection issues.
                var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                builder.TrustServerCertificate = true;
                var robustConnectionString = builder.ConnectionString;

                Logger.Info($"Database Connection String: '{robustConnectionString}'"); // Log the connection string
                DatabaseManager.ConnectionString = robustConnectionString; // Set the static property
                optionsBuilder.UseSqlServer(robustConnectionString);

                Setting.PasswordHash = configuration["PasswordHash"];

                using (var dbContext = new O2JamDbContext(optionsBuilder.Options))
                {
                    var netServer = new NetServer(Setting, dbContext);
                    netServer.Start();

                    Logger.Info("Server started successfully. Press any key to stop.");
                    Console.ReadKey();

                    // 【第三处修改】在主服务器停止时，也停止我们的游戏HTTP服务器
                    netServer.Stop();
                    _gameHttpServer?.Stop();
                }
            }

            LogProvider.Stop();
            Logger.Info("Server stopped.");
        }

        // --- 日志打印方法 LogProviderOnOnLogWrite 完全保持不变 ---
        private static void LogProviderOnOnLogWrite(object sender, LogWriteEventArgs e)
        {
            ConsoleColor consoleColor = ConsoleColor.Gray;
            switch (e.Log.LogLevel)
            {
                case LogLevel.Debug: consoleColor = ConsoleColor.Yellow; break;
                case LogLevel.Info: consoleColor = ConsoleColor.Cyan; break;
                case LogLevel.Error: consoleColor = ConsoleColor.Red; break;
            }

            if (e.Log.Tag is NetPacket packet)
            {
                switch (packet.Source)
                {
                    case PacketSource.Server: consoleColor = ConsoleColor.Green; break;
                    case PacketSource.Client: consoleColor = ConsoleColor.Magenta; break;
                }
            }

            lock (ConsoleLock)
            {
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(e.Log);
                Console.ResetColor();
            }
        }
    }
}