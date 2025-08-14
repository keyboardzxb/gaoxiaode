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
                Logger.Debug($"Player '{client.Character.Name}' is already in the lobby.");
                // 即使已经在Lobby，也可能需要一个响应来防止客户端卡住
                client.Send(new StreamBuffer().GetAllBytes(), PacketId.RoomBackButtonRes);
                return;
            }

            Arrowgene.O2Jam.Server.State.Room room = Lobby.GetRoom(roomId);
            if (room == null)
            {
                Logger.Error($"Player '{client.Character.Name}' tried to leave non-existent Room {roomId}. Forcing state reset.");
                client.CurrentRoomId = -1; // 强制重置客户端状态
                client.Send(new StreamBuffer().GetAllBytes(), PacketId.RoomBackButtonRes);
                return;
            }

            Logger.Info($"Player '{client.Character.Name}' is leaving Room {roomId} ('{room.Name}').");

            // 先将当前客户端移出房间，并重置其房间ID
            // 这是最核心的服务器状态更新
            room.RemovePlayer(client);

            // 检查离开的是否是房主
            if (room.Host == client)
            {
                Logger.Info($"Host '{client.Character.Name}' has left. Dissolving room {roomId}.");

                // 房主离开，需要通知房间内所有其他玩家，他们被“踢出”了
                var otherPlayers = room.Players.Values.ToList();
                foreach (var playerInRoom in otherPlayers)
                {
                    // 重置其他玩家的房间状态
                    playerInRoom.CurrentRoomId = -1;

                    // 发送一个“被踢出”的通知包给其他玩家
                    // O2Jam通常有一个专门的包来处理这种情况，这里我们用一个通用的响应。
                    // 您未来可以抓包分析，找到确切的“被房主踢出”的包并在这里实现。
                    // 现在这个响应将简单地告诉客户端返回大厅。
                    IBuffer kickPacket = new StreamBuffer();
                    // kickPacket.WriteCString("Room closed by host."); // 根据具体协议可能需要消息
                    playerInRoom.Send(kickPacket.GetAllBytes(), PacketId.RoomBackButtonRes); // 使用相同的返回大厅包
                    Logger.Info($"Notifying player '{playerInRoom.Character.Name}' that room is closed.");
                }

                // 从Lobby中彻底移除这个房间
                Lobby.RemoveRoom(roomId);
            }

            // 最后，给发起离开请求的客户端发送成功的响应
            client.Send(new StreamBuffer().GetAllBytes(), PacketId.RoomBackButtonRes);
            Logger.Info($"Player '{client.Character.Name}' has successfully returned to lobby.");
        }
    }
}