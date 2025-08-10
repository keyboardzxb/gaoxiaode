using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Packet;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    // ÐÞÕý£º¼Ì³Ð×Ô PacketHandler
    public class UnknownHandle : PacketHandler
    {
        public override PacketId Id => PacketId.Unknown;
        public override void Handle(Client client, NetPacket packet) { }
    }
}