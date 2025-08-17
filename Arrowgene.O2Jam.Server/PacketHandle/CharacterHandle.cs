using Arrowgene.Buffers;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.Models;
using Arrowgene.Logging;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class CharacterHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(CharacterHandle));

        public override PacketId Id => PacketId.CharacterReq;

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

            res.WriteInt32(0); // Status Code, 0 for success
            res.WriteCString(character.Name); // Character Name from DB
            res.WriteByte((byte)character.Gender); // Gender from DB
            res.WriteInt32(0); // Unknown field, keep as 0
            res.WriteInt32(0); // Unknown field, keep as 0
            res.WriteInt32(character.Gems); // Changed from character.Cash

            // Player Stats
            res.WriteInt32(character.Level); // Level from DB
            res.WriteInt32(0); // Unknown field
            res.WriteInt32(0); // Unknown field
            res.WriteInt32(0); // Unknown field
            res.WriteInt32(character.Exp); // Exp from DB
            res.WriteInt32(0); // Unknown field
            res.WriteByte(0);  // Unknown field

            // Equipped Items Block 1 (11 items)
            res.WriteInt32(character.Instrument);
            res.WriteInt32(character.Hat);
            res.WriteInt32(character.Props);
            res.WriteInt32(character.Glove);
            res.WriteInt32(character.Necklace);
            res.WriteInt32(character.Top);
            res.WriteInt32(character.Bottom);
            res.WriteInt32(character.Glasses);
            res.WriteInt32(character.Earring);
            res.WriteInt32(character.CostumeProps);
            res.WriteInt32(character.Shoes);

            res.WriteInt32(35); // val2, appears to be a static value from reference.

            // Equipped Items Block 2 (5 items)
            res.WriteInt32(character.Wing);
            res.WriteInt32(character.InstrumentProps);
            res.WriteInt32(character.Pet);
            res.WriteInt32(character.HairAccessory);
            res.WriteInt32(character.SetAccessory);

            // My Bag (Inventory)
            res.WriteInt32(1);
            res.WriteInt32(0);

            // Null terminator for Bag
            res.WriteInt32(0);

            // Present box
            res.WriteInt16(0);

            // Some static fields
            res.WriteInt16(0);
            res.WriteInt32(0);
            res.WriteInt32(0);

            // Second Cash Point value
            res.WriteInt32(character.Cash);

            // Item rings
            res.WriteInt32(0);

            // Penalty info
            res.WriteInt16((short)character.PenaltyCount);
            res.WriteInt16((short)character.PenaltyLevel);

            client.Send(res.GetAllBytes(), PacketId.CharacterRes);

            Logger.Info($"Sent character data for '{character.Name}' to the client.");
        }
    }
}