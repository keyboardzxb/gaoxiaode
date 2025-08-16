// Arrowgene.O2Jam.Server/PacketHandle/LoginHandle.cs
using Arrowgene.Buffers;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Data;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.Logging;
using System;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class LoginHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(LoginHandle));
        private readonly Setting _setting;

        public LoginHandle(Setting setting)
        {
            _setting = setting;
        }

        public override PacketId Id => PacketId.LoginReq;

        public override void Handle(Client client, NetPacket packet)
        {
            IBuffer buffer = packet.CreateReadBuffer();
            buffer.Position = 25;
            string username = buffer.ReadCString();
            string password = buffer.ReadCString();

            Logger.Info($"Login attempt from game client: User='{username}'");

            string passwordForDb = password;
            if (string.Equals(_setting.PasswordHash, "MD5", StringComparison.OrdinalIgnoreCase))
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    var inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                    var hashBytes = md5.ComputeHash(inputBytes);
                    passwordForDb = Convert.ToHexString(hashBytes).ToLower();
                }
                Logger.Info($"Password hashing is set to MD5. Hashed password: {passwordForDb}");
            }
            else
            {
                Logger.Info("Password hashing is set to None. Using plain text password.");
            }

            Account account;
            try
            {
                account = DatabaseManager.GetAccount(username, passwordForDb);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                Logger.Error("DATABASE CONNECTION FAILED. Please check your connection string in appsettings.json.");
                Logger.Error("Verify the following:");
                Logger.Error("  1. The server name/IP address is correct.");
                Logger.Error("  2. The SQL Server instance is running.");
                Logger.Error("  3. A firewall is not blocking port 1433 (or your custom SQL port).");
                Logger.Error("  4. SQL Server is configured to allow remote connections.");
                Logger.Error($"  Underlying error: {ex.Message}");
                // Send a generic failure message to the client
                IBuffer failureRes = new StreamBuffer();
                failureRes.WriteInt32(1); // Generic login failure code
                client.Send(failureRes.GetAllBytes(), PacketId.LoginRes);
                return;
            }
            catch (Exception ex)
            {
                Logger.Error($"An unexpected error occurred during login for user '{username}': {ex.Message}");
                // Send a generic failure message to the client
                IBuffer failureRes = new StreamBuffer();
                failureRes.WriteInt32(1); // Generic login failure code
                client.Send(failureRes.GetAllBytes(), PacketId.LoginRes);
                return;
            }

            if (account == null)
            {
                Logger.Info($"Login failed for user: '{username}'. Invalid credentials.");
                IBuffer failureRes = new StreamBuffer();
                failureRes.WriteInt32(1);
                client.Send(failureRes.GetAllBytes(), PacketId.LoginRes);
                return;
            }

            // ***** 决定性的修正 - 开始 *****

            // 在成功验证账户后，我们必须立刻加载与该账户关联的角色数据。
            Character character = DatabaseManager.GetCharacterByAccountId(account.Id);
            if (character == null)
            {
                // 这是一个严重错误，意味着数据库中存在账户但没有对应的角色。
                Logger.Error($"FATAL: Character data not found for account '{username}' (Id: {account.Id}). Disconnecting.");
                client.Close(); // 如果找不到角色，则断开连接。
                return;
            }

            Logger.Info($"Successfully loaded User '{username}' and Character '{character.Name}'.");

            // 将账户和角色信息都保存在客户端的会话状态中，以供后续的 CharacterHandle 使用。
            client.Account = account;
            client.Character = character;

            // ***** 决定性的修正 - 结束 *****


            // 构建并发送登录成功的回应包 (这部分逻辑保持不变)
            IBuffer successRes = new StreamBuffer();
            successRes.WriteInt32(0);
            successRes.WriteByte(32);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteCString(DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss"));
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteUInt16(0);
            successRes.WriteByte(1);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteCString("keyboard");
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteUInt16(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            client.Send(successRes.GetAllBytes(), PacketId.LoginRes);
        }
    }
}
