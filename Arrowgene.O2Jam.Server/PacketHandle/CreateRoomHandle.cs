// Arrowgene.O2Jam.Server/PacketHandle/CreateRoomHandle.cs
using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.State;
using System.Text;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class CreateRoomHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(CreateRoomHandle));
        private static readonly Encoding KoreanEncoding = Encoding.GetEncoding("EUC-KR", new EncoderReplacementFallback("?"), new DecoderReplacementFallback("?"));

        public override PacketId Id => PacketId.CreateRoomReq;

        public override void Handle(Client client, NetPacket packet)
        {
            IBuffer req = new StreamBuffer(packet.Data);
            string roomName = req.ReadCString(KoreanEncoding);

            // 【健壮性修正】如果读取的房间名是空的，就生成一个默认名字
            if (string.IsNullOrWhiteSpace(roomName))
            {
                roomName = $"{client.Character.Name}'s Room";
                Logger.Debug($"Received empty room name from client. Defaulting to '{roomName}'.");
            }

            var newRoom = Lobby.CreateRoom(roomName, client);

            if (newRoom == null)
            {
                Logger.Error($"Player '{client.Character.Name}' failed to create a room.");
                return;
            }

            Logger.Info($"Player '{client.Character.Name}' created Room {newRoom.Id} ('{newRoom.Name}').");

            IBuffer res = new StreamBuffer();
            res.WriteInt32(0); // Success
            res.WriteInt32(newRoom.Id);
            res.WriteInt16(0);
            client.Send(res.GetAllBytes(), PacketId.CreateRoomRes);

            RoomListHandle.BroadcastNewRoom(newRoom, client);
        }
    }
}