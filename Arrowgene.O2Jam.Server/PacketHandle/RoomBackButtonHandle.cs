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
                // ��ʹ�Ѿ���Lobby��Ҳ������Ҫһ����Ӧ����ֹ�ͻ��˿�ס
                client.Send(new StreamBuffer().GetAllBytes(), PacketId.RoomBackButtonRes);
                return;
            }

            Arrowgene.O2Jam.Server.State.Room room = Lobby.GetRoom(roomId);
            if (room == null)
            {
                Logger.Error($"Player '{client.Character.Name}' tried to leave non-existent Room {roomId}. Forcing state reset.");
                client.CurrentRoomId = -1; // ǿ�����ÿͻ���״̬
                client.Send(new StreamBuffer().GetAllBytes(), PacketId.RoomBackButtonRes);
                return;
            }

            Logger.Info($"Player '{client.Character.Name}' is leaving Room {roomId} ('{room.Name}').");

            // �Ƚ���ǰ�ͻ����Ƴ����䣬�������䷿��ID
            // ��������ĵķ�����״̬����
            room.RemovePlayer(client);

            // ����뿪���Ƿ��Ƿ���
            if (room.Host == client)
            {
                Logger.Info($"Host '{client.Character.Name}' has left. Dissolving room {roomId}.");

                // �����뿪����Ҫ֪ͨ����������������ң����Ǳ����߳�����
                var otherPlayers = room.Players.Values.ToList();
                foreach (var playerInRoom in otherPlayers)
                {
                    // ����������ҵķ���״̬
                    playerInRoom.CurrentRoomId = -1;

                    // ����һ�������߳�����֪ͨ�����������
                    // O2Jamͨ����һ��ר�ŵİ��������������������������һ��ͨ�õ���Ӧ��
                    // ��δ������ץ���������ҵ�ȷ�еġ��������߳����İ���������ʵ�֡�
                    // ���������Ӧ���򵥵ظ��߿ͻ��˷��ش�����
                    IBuffer kickPacket = new StreamBuffer();
                    // kickPacket.WriteCString("Room closed by host."); // ���ݾ���Э�������Ҫ��Ϣ
                    playerInRoom.Send(kickPacket.GetAllBytes(), PacketId.RoomBackButtonRes); // ʹ����ͬ�ķ��ش�����
                    Logger.Info($"Notifying player '{playerInRoom.Character.Name}' that room is closed.");
                }

                // ��Lobby�г����Ƴ��������
                Lobby.RemoveRoom(roomId);
            }

            // ��󣬸������뿪����Ŀͻ��˷��ͳɹ�����Ӧ
            client.Send(new StreamBuffer().GetAllBytes(), PacketId.RoomBackButtonRes);
            Logger.Info($"Player '{client.Character.Name}' has successfully returned to lobby.");
        }
    }
}