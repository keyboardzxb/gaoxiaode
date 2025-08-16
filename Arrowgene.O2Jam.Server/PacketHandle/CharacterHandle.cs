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

            // Diagnostic Logging: Print all character data before sending
            Logger.Info("--- Sending Character Data ---");
            Logger.Info($"Name: {character.Name}");
            Logger.Info($"Gender: {character.Gender}");
            Logger.Info($"Gems: {character.Gems}");
            Logger.Info($"Cash (MCash): {character.Cash}");
            Logger.Info($"Level: {character.Level}");
            Logger.Info($"Exp: {character.Exp}");
            Logger.Info($"Penalty Count: {character.PenaltyCount}");
            Logger.Info($"Penalty Level: {character.PenaltyLevel}");
            Logger.Info("--- Equipped Items ---");
            Logger.Info($"Instrument (Equip1): {character.Instrument}");
            Logger.Info($"Hat (Equip2): {character.Hat}");
            Logger.Info($"HairAccessory (Equip3): {character.HairAccessory}");
            Logger.Info($"Glasses (Equip4): {character.Glasses}");
            Logger.Info($"Shoes (Equip5): {character.Shoes}");
            Logger.Info($"Top (Equip6): {character.Top}");
            Logger.Info($"Bottom (Equip7): {character.Bottom}");
            Logger.Info($"Glove (Equip8): {character.Glove}");
            Logger.Info($"Necklace (Equip9): {character.Necklace}");
            Logger.Info($"SetAccessory (Equip10): {character.SetAccessory}");
            Logger.Info($"Wing (Equip11): {character.Wing}");
            Logger.Info($"Earring/Face (Equip12): {character.Earring}");
            Logger.Info($"Pet (Equip13): {character.Pet}");
            Logger.Info($"Props (Equip14): {character.Props}");
            Logger.Info($"CostumeProps (Equip15): {character.CostumeProps}");
            Logger.Info($"InstrumentProps (Equip16): {character.InstrumentProps}");
            Logger.Info("--------------------------");

            IBuffer res = new StreamBuffer();

            // We will now build the response packet with safe, hardcoded values for diagnosis.

            res.WriteInt32(0); // Status Code, 0 for success
            res.WriteCString("TestUser"); // Character Name
            res.WriteByte(1); // Gender (1=male)
            res.WriteInt32(0); // Unknown field
            res.WriteInt32(0); // Unknown field
            res.WriteInt32(10000); // Gems

            // Player Stats
            res.WriteInt32(1); // Level
            res.WriteInt32(0); // Unknown field
            res.WriteInt32(0); // Unknown field
            res.WriteInt32(0); // Unknown field
            res.WriteInt32(0); // Exp
            res.WriteInt32(0); // Unknown field
            res.WriteByte(0);  // Unknown field

            // Equipped Items Block 1 (11 items) - Using default male items
            res.WriteInt32(0);   // Instrument
            res.WriteInt32(7);   // Hat (Hair)
            res.WriteInt32(0);   // Props
            res.WriteInt32(0);   // Glove
            res.WriteInt32(0);   // Necklace
            res.WriteInt32(79);  // Top
            res.WriteInt32(106); // Bottom
            res.WriteInt32(0);   // Glasses
            res.WriteInt32(35);  // Earring (Face)
            res.WriteInt32(0);   // CostumeProps
            res.WriteInt32(0);   // Shoes

            res.WriteInt32(35); // val2, Face ID confirmation

            // Equipped Items Block 2 (5 items)
            res.WriteInt32(0); // Wing
            res.WriteInt32(0); // InstrumentProps
            res.WriteInt32(0); // Pet
            res.WriteInt32(0); // HairAccessory
            res.WriteInt32(0); // SetAccessory

            // My Bag (Inventory)
            res.WriteInt32(1); // Item count + 1 for the empty space
            res.WriteInt32(0); // The empty space item id

            // Null terminator for Bag
            res.WriteInt32(0); // null

            // Present box
            res.WriteInt16(0); // Number of gifts

            // Some static fields from reference file after present box
            res.WriteInt16(0);
            res.WriteInt32(0);
            res.WriteInt32(0);

            // Second Cash Point value, seems to be a confirmation.
            res.WriteInt32(1000); // MCash

            // Item rings
            res.WriteInt32(0); // Number of ring types owned

            // Penalty info
            res.WriteInt16(0); // Penalty Count
            res.WriteInt16(0); // Penalty Level

            // Send the fully constructed packet to the client
            client.Send(res.GetAllBytes(), PacketId.CharacterRes);

            Logger.Info($"Sent DIAGNOSTIC character data for '{character.Name}' to the client.");
        }
    }
}