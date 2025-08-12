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
using System; // 新增 System 引用

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class RoomListHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(RoomListHandle));
        private static readonly Encoding KoreanEncoding = Encoding.GetEncoding("EUC-KR", new EncoderReplacementFallback(""), new DecoderReplacementFallback(""));

        // 【新增辅助方法】用于写入固定长度、自动用 null 补齐的字符串
        private static void WriteFixedString(IBuffer buffer, string text, int fixedLength, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(text);
            byte[] fixedBuffer = new byte[fixedLength];
            int lengthToCopy = Math.Min(bytes.Length, fixedLength);
            Array.Copy(bytes, 0, fixedBuffer, 0, lengthToCopy);
            buffer.WriteBytes(fixedBuffer);
        }

        // 【核心修正】彻底重写数据包构建方法，严格按照反编译文件的标准
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

        // --- 以下是您文件中原有的、无需修改的方法，保持原样即可 ---

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
            // 这里的逻辑可以根据您的需要未来再实现
        }

        public static void ForceKickPlayer(Client player, string reason)
        {
            // 这里的逻辑可以根据您的需要未来再实现
        }

        public static void SendUnlockPacket(Client client)
        {
            // 注意: O2Jam的协议中，这个包似乎是空的，但必须发送
            IBuffer emptyListRes = new StreamBuffer();
            client.Send(emptyListRes.GetAllBytes(), PacketId.RoomListRes);

            // 这个未知的响应包也可能对客户端UI解锁很重要
            IBuffer res2 = new StreamBuffer();
            res2.WriteInt32(2); res2.WriteInt32(3); res2.WriteInt32(0); res2.WriteInt32(0); res2.WriteUInt16(0);
            client.Send(res2.GetAllBytes(), PacketId.UnkRes);
        }
    }
}