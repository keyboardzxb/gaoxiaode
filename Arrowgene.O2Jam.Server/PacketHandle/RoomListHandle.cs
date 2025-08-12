// Arrowgene.O2Jam.Server/PacketHandle/RoomListHandle.cs
using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.State;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System; // ���� System ����

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class RoomListHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(RoomListHandle));
        private static readonly Encoding KoreanEncoding = Encoding.GetEncoding("EUC-KR", new EncoderReplacementFallback(""), new DecoderReplacementFallback(""));

        // ��������������������д��̶����ȡ��Զ��� null ������ַ���
        private static void WriteFixedString(IBuffer buffer, string text, int fixedLength, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(text);
            byte[] fixedBuffer = new byte[fixedLength];
            int lengthToCopy = Math.Min(bytes.Length, fixedLength);
            Array.Copy(bytes, 0, fixedBuffer, 0, lengthToCopy);
            buffer.WriteBytes(fixedBuffer);
        }

        // ������������������д���ݰ������������ϸ��շ������ļ��ı�׼
        private static IBuffer BuildAnnouncePacket(Room room)
        {
            IBuffer buffer = new StreamBuffer();
            buffer.WriteInt32(room.Id);
            WriteFixedString(buffer, room.Name, 32, KoreanEncoding);
            buffer.WriteUInt16(0); // Password Flag
            buffer.WriteUInt16(1); // Status
            buffer.WriteUInt16((ushort)room.Players.Count);
            buffer.WriteUInt16(8); // Max Players
            buffer.WriteUInt16(0); // Song ID
            buffer.WriteUInt16(0); // Unknown
            buffer.WriteInt32(room.Host.Character.Level);
            WriteFixedString(buffer, room.Host.Character.Name, 16, KoreanEncoding);
            return buffer;
        }

        // --- ���������ļ���ԭ�еġ������޸ĵķ���������ԭ������ ---

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
            var recipients = Lobby.GetLobbyClientsExcept(creator);
            if (!recipients.Any())
            {
                Logger.Info($"No other players in lobby to broadcast new room '{newRoom.Name}' to.");
                return;
            }

            Logger.Info($"Broadcasting new room '{newRoom.Name}' to {recipients.Count()} other players.");
            foreach (var client in recipients)
            {
                client.Send(announcePacket.GetAllBytes(), PacketId.AnnounceRoomRes);
            }
        }

        public static void BroadcastRoomClosed(int roomId)
        {
            // ������߼����Ը���������Ҫδ����ʵ��
        }

        public static void ForceKickPlayer(Client player, string reason)
        {
            // ������߼����Ը���������Ҫδ����ʵ��
        }

        public static void SendUnlockPacket(Client client)
        {
            // ע��: O2Jam��Э���У�������ƺ��ǿյģ������뷢��
            IBuffer emptyListRes = new StreamBuffer();
            client.Send(emptyListRes.GetAllBytes(), PacketId.RoomListRes);

            // ���δ֪����Ӧ��Ҳ���ܶԿͻ���UI��������Ҫ
            IBuffer res2 = new StreamBuffer();
            res2.WriteInt32(2); res2.WriteInt32(3); res2.WriteInt32(0); res2.WriteInt32(0); res2.WriteUInt16(0);
            client.Send(res2.GetAllBytes(), PacketId.UnkRes);
        }
    }
}