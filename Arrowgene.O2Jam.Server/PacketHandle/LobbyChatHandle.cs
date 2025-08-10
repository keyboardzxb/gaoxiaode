// 文件路径: Arrowgene.O2Jam.Server/PacketHandle/LobbyChatHandle.cs (最终精确版)
using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class LobbyChatHandle : IPacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(LobbyChatHandle));
        private static readonly Encoding ChineseEncoding;
        private readonly NetServer _netServer;

        // 定义一个固定的角色名字节长度
        private const int FIXED_NICKNAME_LENGTH = 20; // 这是一个猜测值，常见的长度有16, 20, 24

        static LobbyChatHandle()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ChineseEncoding = Encoding.GetEncoding(936); // 简体中文 GB2312
        }

        public LobbyChatHandle(NetServer netServer)
        {
            _netServer = netServer;
        }

        public PacketId Id => PacketId.LobbyChatReq; // 2012

        public void Handle(Client client, NetPacket packet)
        {
            IBuffer req = new StreamBuffer(packet.Data);
            string message = req.ReadCString(ChineseEncoding);
            Logger.Info($"Received Chat from '{client.Character.Name}': {message}");

            // (关键修改) 构建结构精确的广播包
            IBuffer broadcastPacket = new StreamBuffer();

            // 1. 写入发送者名称，作为固定长度、用'\0'填充的字节数组
            byte[] nameBytes = new byte[FIXED_NICKNAME_LENGTH];
            byte[] rawNameBytes = ChineseEncoding.GetBytes(client.Character.Name);
            // 复制真实名字字节，多余的部分自动为0 (即 '\0')
            System.Buffer.BlockCopy(rawNameBytes, 0, nameBytes, 0, rawNameBytes.Length < FIXED_NICKNAME_LENGTH ? rawNameBytes.Length : FIXED_NICKNAME_LENGTH);
            broadcastPacket.WriteBytes(nameBytes);

            // 2. 写入聊天内容，作为标准的C-String（以'\0'结尾）
            broadcastPacket.WriteCString(message, ChineseEncoding);

            byte[] broadcastData = broadcastPacket.GetAllBytes();

            // 广播给所有在大厅的玩家
            var lobbyClients = _netServer.GetAllClients().Where(c => c.CurrentRoomId == -1).ToList();
            Logger.Info($"Broadcasting chat message to {lobbyClients.Count} clients in lobby.");

            foreach (Client recipient in lobbyClients)
            {
                recipient.Send(broadcastData, PacketId.LobbyChatRes); // 2013
            }
        }
    }
}