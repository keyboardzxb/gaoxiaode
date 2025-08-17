using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class RoomApplySkill : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(RoomApplySkill));

        public override PacketId Id => PacketId.RoomUnknown1Req;

        public override void Handle(Client client, NetPacket packet)
        {
            IBuffer buffer = packet.CreateReadBuffer();
            int countApply = 0;
            int arrange = 0;
            int visibility = 0;

            if (buffer.ReadableBytes >= 4)
            {
                countApply = buffer.ReadInt32();
            }
            if (buffer.ReadableBytes >= 4)
            {
                arrange = buffer.ReadInt32();
            }
            if (buffer.ReadableBytes >= 4)
            {
                visibility = buffer.ReadInt32();
            }

            Logger.Info($"[CountApply:{countApply}][Arrange:{arrange}][Visibility:{visibility}]");
            
            IBuffer res = new StreamBuffer();
            res.WriteInt32(countApply);
            res.WriteInt32(arrange);
            res.WriteInt32(visibility);
            client.Send(res.GetAllBytes(), PacketId.RoomUnknown1Res);
            //Res_4024_0x0FB8 = 4024, // 0x0FB8 = 0x005602A0
        }
    }
}