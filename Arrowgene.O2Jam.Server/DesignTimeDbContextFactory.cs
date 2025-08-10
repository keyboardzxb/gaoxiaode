using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Arrowgene.O2Jam.Server.Data;

namespace Arrowgene.O2Jam.Server
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<O2JamDbContext>
    {
        public O2JamDbContext CreateDbContext(string[] args)
        {
            // 这个路径会从我们执行命令的 Server 文件夹，向上返回一级，再进入主项目文件夹，以确保能找到正确的配置文件
            string basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"../Arrowgene.O2Jam"));

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<O2JamDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlite(connectionString);

            return new O2JamDbContext(optionsBuilder.Options);
        }
    }
}