// ÎÄ¼þÂ·¾¶: Arrowgene.O2Jam.Server/PacketHandle/RoomListHandle.cs
using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.State;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class RoomListHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(RoomListHandle));
        private static readonly Encoding KoreanEncoding = Encoding.GetEncoding("EUC-KR", new EncoderReplacementFallback(""), new DecoderReplacementFallback(""));

        public override PacketId Id => PacketId.RoomListReq;

        public override void Handle(Client client, NetPacket packet)
        {
            SendUnlockPacket(client);
            AnnounceAllRoomsTo(client);
        }

        public static void AnnounceAllRoomsTo(Client recipient)
        {
            List<Room> rooms = Lobby.GetRooms();
            if (!rooms.Any()) return;
            Logger.Info($"Announcing {rooms.Count} existing rooms to '{recipient.Character.Name}'.");
            foreach (var room in rooms)
            {
                IBuffer announcePacket = BuildAnnouncePacket(room);
                recipient.Send(announcePacket.GetAllBytes(), PacketId.AnnounceRoomRes);
            }
        }

        public static void BroadcastNewRoom(Room newRoom, Client creator)
        {
            IBuffer announcePacket = BuildAnnouncePacket(newRoom);
            var recipients = Lobby.GetAllClients().Where(c => c.Account != creator.Account && c.CurrentRoomId == -1);
            if (!recipients.Any()) return;
            Logger.Info($"Broadcasting new room '{newRoom.Name}' to {recipients.Count()} other players.");
            foreach (var client in recipients)
            {
                client.Send(announcePacket.GetAllBytes(), PacketId.AnnounceRoomRes);
            }
        }

        public static void SendUnlockPacket(Client client)
        {
            IBuffer emptyListRes = new StreamBuffer();
            emptyListRes.WriteInt32(0);
            client.Send(emptyListRes.GetAllBytes(), PacketId.RoomListRes);

            IBuffer res2 = new StreamBuffer();
            res2.WriteInt32(2); res2.WriteInt32(3); res2.WriteInt32(0); res2.WriteInt32(0); res2.WriteUInt16(0);
            client.Send(res2.GetAllBytes(), PacketId.UnkRes);
        }

        private static IBuffer BuildAnnouncePacket(Room room)
        {
            IBuffer buffer = new StreamBuffer();
            buffer.WriteInt32(room.Id);
            buffer.WriteCString(room.Name, KoreanEncoding);
            buffer.WriteByte(0); // Password Flag
            buffer.WriteByte(1); // Room Status
            buffer.WriteByte((byte)room.Players.Count);
            buffer.WriteByte(8); // Max Players
            buffer.WriteByte((byte)room.Mode);
            buffer.WriteUInt16(0); // Song ID
            buffer.WriteByte((byte)room.Host.Character.Level);
            return buffer;
        }
    }
}