using Arrowgene.O2Jam.Server.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using Xunit;

namespace Arrowgene.O2Jam.Test.Data
{
    public class DatabaseManagerTest
    {
        public DatabaseManagerTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            DatabaseManager.ConnectionString = connectionString;
        }

        [Fact]
        public void TestLogin()
        {
            // For this test to pass, you must have a user in your database
            // with the username 'test' and password 'test'.
            // This user must also have corresponding entries in the Users and Players tables.
            var account = DatabaseManager.GetAccount("test", "test");
            Assert.NotNull(account);

            var character = DatabaseManager.GetCharacterByAccountId(account.Id);
            Assert.NotNull(character);
        }
    }
}
