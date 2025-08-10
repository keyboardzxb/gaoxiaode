// Arrowgene.O2Jam/Program.cs
using System;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Data;
using Microsoft.EntityFrameworkCore; // 新增：必须引入此命名空间

namespace Arrowgene.O2Jam
{
    class Program
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(Program));
        private static readonly object ConsoleLock = new();
        private static readonly Setting Setting = new();

        static void Main(string[] args)
        {
            // --- 日志部分保持不变 ---
            LogProvider.Configure<ServerLogger>(Setting);
            LogProvider.OnLogWrite += LogProviderOnOnLogWrite;
            LogProvider.Start();

            try { DatabaseManager.Initialize(); Logger.Info("Database initialized successfully."); }
            catch (Exception ex) { Logger.Error($"FATAL: Database init failed: {ex.Message}"); Console.ReadKey(); return; }

            try { new HttpRegistrationServer().Start(); }
            catch (Exception ex) { Logger.Error($"FATAL: HTTP Server start failed: {ex.Message}"); Console.ReadKey(); return; }

            if (args.Length == 0)
            {
                // ***** 最终修正点 (开始) *****

                // 1. 创建一个数据库配置选项的构造器。
                var optionsBuilder = new DbContextOptionsBuilder<O2JamDbContext>();

                // 2. 配置这个构造器，告诉它我们要使用SQLite，并且数据库文件是 "o2jam.db"。
                optionsBuilder.UseSqlite("Data Source=o2jam.db");

                // 3. 使用上面配置好的选项(optionsBuilder.Options)来创建DbContext实例。
                //    这就完全满足了构造函数的要求。
                using (var dbContext = new O2JamDbContext(optionsBuilder.Options))
                {
                    // 将创建好的dbContext实例传递给NetServer，后续逻辑保持不变。
                    var netServer = new NetServer(Setting,dbContext);
                    netServer.Start();

                    Logger.Info("Server started successfully. Press any key to stop.");
                    Console.ReadKey();
                    netServer.Stop();
                }

                // ***** 最终修正点 (结束) *****
            }

            LogProvider.Stop();
            Logger.Info("Server stopped.");
        }

        // --- 日志打印方法 LogProviderOnOnLogWrite 保持不变 ---
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