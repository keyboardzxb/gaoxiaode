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

            // Build the packet exactly matching the user's reference code.
            res.WriteInt32(0); // Status Code, 0 for success
            res.WriteCString(character.Name);
            res.WriteByte((byte)character.Gender);
            res.WriteInt32(0); // Unknown
            res.WriteInt32(0); // Unknown
            res.WriteInt32(character.Cash); // First currency field, as per reference

            // Player Stats
            res.WriteInt32(character.Level);
            res.WriteInt32(0); // Unknown
            res.WriteInt32(0); // Unknown
            res.WriteInt32(0); // Unknown
            res.WriteInt32(character.Exp);
            res.WriteInt32(0); // Unknown
            res.WriteByte(0);  // Unknown

            // Equipped Items Block 1 (11 items) - As Int32, per reference
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

            res.WriteInt32(35); // Hardcoded value, as per reference

            // Equipped Items Block 2 (5 items)
            res.WriteInt32(character.Wing);
            res.WriteInt32(character.InstrumentProps);
            res.WriteInt32(character.Pet);
            res.WriteInt32(character.HairAccessory);
            res.WriteInt32(character.SetAccessory);

            // My Bag (Inventory)
            res.WriteInt32(1);
            res.WriteInt32(0);
            res.WriteInt32(0);

            // Present box
            res.WriteInt16(0);

            // Static fields
            res.WriteInt16(0);
            res.WriteInt32(0);
            res.WriteInt32(0);

            // Second Cash Point value
            res.WriteInt32(character.Cash); // Second currency field, as per reference

            // Item rings
            res.WriteInt32(0);

            // Penalty info
            res.WriteInt16((short)character.PenaltyCount);
            res.WriteInt16((short)character.PenaltyLevel);

            // Send the fully constructed packet to the client
            client.Send(res.GetAllBytes(), PacketId.CharacterRes);

            Logger.Info($"Sent character data for '{character.Name}' to the client, following reference code.");
        }
    }
}