using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Packet;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    // �������̳��� PacketHandler
    public class UnknownHandle : PacketHandler
    {
        public override PacketId Id => PacketId.Unknown;
        public override void Handle(Client client, NetPacket packet) { }
    }
}