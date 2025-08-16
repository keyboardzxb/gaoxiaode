using Arrowgene.Buffers;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging; // Assuming you have this
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.Data;
using Arrowgene.Logging; // To get the Character class

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class CharacterHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(CharacterHandle));

        public override PacketId Id => PacketId.CharacterReq;

        public override void Handle(Client client, NetPacket packet)
        {
            // At this point, LoginHandle should have already authenticated the user
            // and loaded the character data into the client session.
            if (client.Account == null || client.Character == null)
            {
                Logger.Error($"Character request received but no account/character data in session. Disconnecting client.");
                client.Close();
                return;
            }

            Character character = client.Character;
            IBuffer res = new StreamBuffer();

            // We will now build the response packet EXACTLY like the reference data file,
            // but using data from the database via the 'character' object.

            res.WriteInt32(0); // Status Code, 0 for success
            res.WriteCString(character.Name); // Character Name from DB
            res.WriteByte((byte)character.Gender); // Gender from DB
            res.WriteInt32(0); // Unknown field, keep as 0
            res.WriteInt32(0); // Unknown field, keep as 0
            res.WriteInt32(character.Gems); // Gem Point from DB

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

            res.WriteInt32(character.Earring); // This was hardcoded to 35. It should be the face ID, which is stored in Earring (Equip12).

            // Equipped Items Block 2 (5 items)
            res.WriteInt32(character.Wing);
            res.WriteInt32(character.InstrumentProps);
            res.WriteInt32(character.Pet);
            res.WriteInt32(character.HairAccessory);
            res.WriteInt32(character.SetAccessory);

            // My Bag (Inventory)
            // TODO: This section needs to be implemented to read from an inventory table.
            // For now, we send an empty inventory as per reference file comments.
            res.WriteInt32(1); // Item count + 1 for the empty space
            res.WriteInt32(0); // The empty space item id

            // Null terminator for Bag
            res.WriteInt32(0); // null

            // Present box
            // TODO: This section needs to be implemented to read from a gift table.
            // For now, we send an empty present box.
            res.WriteInt16(0); // Number of gifts

            // Some static fields from reference file after present box
            res.WriteInt16(0);
            res.WriteInt32(0);
            res.WriteInt32(0);

            // Second Cash Point value, seems to be a confirmation.
            res.WriteInt32(character.Cash);

            // Item rings
            // TODO: This section needs to be implemented to read from a rings table.
            // For now, we send zero rings.
            res.WriteInt32(0); // Number of ring types owned

            // Penalty info
            res.WriteInt16((short)character.PenaltyCount); // Penalty Count from DB
            res.WriteInt16((short)character.PenaltyLevel); // Penalty Level from DB

            // Send the fully constructed packet to the client
            client.Send(res.GetAllBytes(), PacketId.CharacterRes);

            Logger.Info($"Sent character data for '{character.Name}' to the client.");
        }
    }
}