using Arrowgene.Buffers;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Packet;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    // 这个新文件用于精确处理客户端进入大厅的 0x2819 请求
    public class EnterLobbyHandle : PacketHandler
    {
        public override PacketId Id => PacketId.EnterLobbyRequest; // 对应我们新增的 10265

        public override void Handle(Client client, NetPacket packet)
        {
            IBuffer buffer = new StreamBuffer();

            // 我们100%复刻抓包数据中 0x1C00 包的内容
            buffer.WriteUInt32(1);      // 当前人数
            buffer.WriteUInt32(100);    // 最大人数
            buffer.WriteByte(1);       // 未知字节1
            buffer.WriteByte(0);       // 未知字节2

            // 使用我们新增的 LobbyInfoResponse (7168) 作为ID创建数据包
            var responsePacket = new NetPacket(PacketId.LobbyInfoResponse, buffer.GetAllBytes());

            // 将这个至关重要的响应发送给客户端
            client.Send(responsePacket);
        }
    }
}