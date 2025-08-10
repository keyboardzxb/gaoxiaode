// 文件路径: Arrowgene.O2Jam.Server/PacketHandle/RoomBackButtonHandle.cs
using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.State;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class RoomBackButtonHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(RoomBackButtonHandle));

        public override PacketId Id => PacketId.RoomBackButtonReq;

        public override void Handle(Client client, NetPacket packet)
        {
            // 注意：这里我们只处理离开房间的逻辑，广播由其他Handle负责
            Lobby.RemovePlayerFromCurrentRoom(client);
            Logger.Info($"Player '{client.Character.Name}' left room.");

            IBuffer res = new StreamBuffer();
            client.Send(res.GetAllBytes(), PacketId.RoomBackButtonRes);

            // 玩家回到大厅，需要再次解锁UI并同步房间
            RoomListHandle.SendUnlockPacket(client);
            RoomListHandle.AnnounceAllRoomsTo(client);
        }
    }
}