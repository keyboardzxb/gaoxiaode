using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.State;
using System.Linq;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class RoomBackButtonHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(RoomBackButtonHandle));

        public override PacketId Id => PacketId.RoomBackButtonReq;

        public override void Handle(Client client, NetPacket packet)
        {
            int roomId = client.CurrentRoomId;
            if (roomId == -1)
            {
                Logger.Debug($"Player '{client.Character.Name}' tried to leave a room, but they are already in the lobby.");
                return;
            }

            // ���� Lobby.GetRoom(roomId) ���Ա���ȷ����
            Room room = Lobby.GetRoom(roomId);
            if (room == null)
            {
                Logger.Error($"Player '{client.Character.Name}' tried to leave Room {roomId}, but the room does not exist. Forcing state reset.");
                client.CurrentRoomId = -1;
                return;
            }

            Logger.Info($"Player '{client.Character.Name}' is leaving Room {roomId}.");

            // ���һ���뿪���Ƿ���
            if (room.Host == client)
            {
                Logger.Info($"Host '{client.Character.Name}' is leaving. Room {roomId} ('{room.Name}') will be dissolved.");

                var playersInRoom = room.Players.Values.ToList();
                foreach (var playerInRoom in playersInRoom)
                {
                    Lobby.RemovePlayerFromRoom(playerInRoom, roomId);
                    if (playerInRoom != client)
                    {
                        // ����Ҫʵ�� ForceKickPlayer ������֪ͨ�ͻ��˱��߳�
                        // RoomListHandle.ForceKickPlayer(playerInRoom, "�������뿪�������ѽ�ɢ��");
                    }
                }

                Lobby.RemoveRoom(roomId);
                // ����Ҫʵ�� BroadcastRoomClosed ������֪ͨ������ҷ���ر�
                // RoomListHandle.BroadcastRoomClosed(roomId);
            }
            // ��������뿪������ͨ���
            else
            {
                Lobby.RemovePlayerFromRoom(client, roomId);
            }

            client.Send(new StreamBuffer().GetAllBytes(), PacketId.RoomBackButtonRes);
            Logger.Info($"Player '{client.Character.Name}' has successfully returned to lobby.");
        }
    }
}