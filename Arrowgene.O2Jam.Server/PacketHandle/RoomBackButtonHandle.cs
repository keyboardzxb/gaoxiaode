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

            // 现在 Lobby.GetRoom(roomId) 可以被正确调用
            Room room = Lobby.GetRoom(roomId);
            if (room == null)
            {
                Logger.Error($"Player '{client.Character.Name}' tried to leave Room {roomId}, but the room does not exist. Forcing state reset.");
                client.CurrentRoomId = -1;
                return;
            }

            Logger.Info($"Player '{client.Character.Name}' is leaving Room {roomId}.");

            // 情况一：离开者是房主
            if (room.Host == client)
            {
                Logger.Info($"Host '{client.Character.Name}' is leaving. Room {roomId} ('{room.Name}') will be dissolved.");

                var playersInRoom = room.Players.Values.ToList();
                foreach (var playerInRoom in playersInRoom)
                {
                    Lobby.RemovePlayerFromRoom(playerInRoom, roomId);
                    if (playerInRoom != client)
                    {
                        // 您需要实现 ForceKickPlayer 方法来通知客户端被踢出
                        // RoomListHandle.ForceKickPlayer(playerInRoom, "房主已离开，房间已解散。");
                    }
                }

                Lobby.RemoveRoom(roomId);
                // 您需要实现 BroadcastRoomClosed 方法来通知大厅玩家房间关闭
                // RoomListHandle.BroadcastRoomClosed(roomId);
            }
            // 情况二：离开者是普通玩家
            else
            {
                Lobby.RemovePlayerFromRoom(client, roomId);
            }

            client.Send(new StreamBuffer().GetAllBytes(), PacketId.RoomBackButtonRes);
            Logger.Info($"Player '{client.Character.Name}' has successfully returned to lobby.");
        }
    }
}