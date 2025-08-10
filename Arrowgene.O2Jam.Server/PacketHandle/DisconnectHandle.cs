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

        // �޸Ĺ��캯�������� Channel ����
        public DisconnectHandle(Channel channel)
        {
            _channel = channel;
        }

        public override void Handle(Client client, NetPacket packet)
        {
            // ��Ƶ�������з������Ƴ��ÿͻ���
            _channel.RemoveClient(client);

            // ���跢����Ӧ����Ϊ�ͻ����Ѿ��Ͽ�
        }
    }
}