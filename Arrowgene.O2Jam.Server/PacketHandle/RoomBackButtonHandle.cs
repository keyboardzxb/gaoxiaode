// �ļ�·��: Arrowgene.O2Jam.Server/PacketHandle/RoomBackButtonHandle.cs
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
            // ע�⣺��������ֻ�����뿪������߼����㲥������Handle����
            Lobby.RemovePlayerFromCurrentRoom(client);
            Logger.Info($"Player '{client.Character.Name}' left room.");

            IBuffer res = new StreamBuffer();
            client.Send(res.GetAllBytes(), PacketId.RoomBackButtonRes);

            // ��һص���������Ҫ�ٴν���UI��ͬ������
            RoomListHandle.SendUnlockPacket(client);
            RoomListHandle.AnnounceAllRoomsTo(client);
        }
    }
}