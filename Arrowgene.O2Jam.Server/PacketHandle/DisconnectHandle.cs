// Arrowgene.O2Jam.Server/PacketHandle/DisconnectHandle.cs
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.State;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class DisconnectHandle : PacketHandler
    {
        private readonly Channel _channel;
        public override PacketId Id => PacketId.DisconnectReq;

        // 修改构造函数，接收 Channel 对象
        public DisconnectHandle(Channel channel)
        {
            _channel = channel;
        }

        public override void Handle(Client client, NetPacket packet)
        {
            // 从频道和所有房间中移除该客户端
            _channel.RemoveClient(client);

            // 无需发送响应，因为客户端已经断开
        }
    }
}