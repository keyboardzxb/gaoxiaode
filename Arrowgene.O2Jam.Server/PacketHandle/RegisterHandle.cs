// Arrowgene.O2Jam.Server/PacketHandle/RegisterHandle.cs (最终修正版)
using Arrowgene.Buffers;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Data;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.Logging;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class RegisterHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(RegisterHandle));
        private readonly Setting _setting;

        public RegisterHandle(Setting setting)
        {
            _setting = setting;
        }

        public override PacketId Id => PacketId.RegisterReq;

        public override void Handle(Client client, NetPacket packet)
        {
            IBuffer buffer = packet.CreateReadBuffer();

            // --- vvv 核心修正 vvv ---
            // 采用与 LoginHandle 完全相同的解析方式，跳过头部的无用数据
            buffer.Position = 15;
            ushort a = buffer.ReadUInt16();
            if (a > 2)
            {
                // 这部分逻辑是为了与客户端完全对齐，即使现在用不到
                buffer.ReadBytes(a - 2);
            }

            string username = buffer.ReadCString();
            string password = buffer.ReadCString();
            // --- ^^^ 核心修正结束 ^^^ ---

            Logger.Info($"Registration attempt for user: '{username}' with password of length: {password.Length}");

            bool success = DatabaseManager.RegisterAccount(username, password, _setting);

            IBuffer res = new StreamBuffer();
            if (success)
            {
                Logger.Info($"User '{username}' registered successfully.");
                res.WriteInt32(0); // 0 = 成功
            }
            else
            {
                Logger.Info($"Registration failed for user: '{username}'. Username might already exist.");
                res.WriteInt32(1); // 1 = 用户名已存在
            }

            client.Send(res.GetAllBytes(), PacketId.RegisterRes);
        }
    }
}