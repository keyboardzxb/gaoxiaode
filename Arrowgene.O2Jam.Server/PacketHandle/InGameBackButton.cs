using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class InGameBackButton : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InGameBackButton));

        public override PacketId Id => PacketId.InGameBackButtonReq;

        public override void Handle(Client client, NetPacket packet)

        {
            if (client.Account == null || client.Character == null)
            {
                Logger.Error($"Character request received but no account/character data in session. Disconnecting client.");
                client.Close();
                return;
            }

            Character character = client.Character;
            IBuffer res = new StreamBuffer();
            res.WriteByte(0);
            res.WriteInt32(character.Level);
            res.WriteInt32(character.Exp); // Exp from DB
            res.WriteByte(0);
            res.WriteByte(0);
            res.WriteByte(0);
            res.WriteByte(0);
            res.WriteByte(0);
            res.WriteByte(0);
            res.WriteByte(0);
            res.WriteByte(0);
            res.WriteByte(0);
            res.WriteByte(0);
            client.Send(res.GetAllBytes(), PacketId.InGameBackButtonRes);
            //Res_4022_0x0FB6 = 4022, // 0x0FB6 = 0x00560160
        }
    }
}