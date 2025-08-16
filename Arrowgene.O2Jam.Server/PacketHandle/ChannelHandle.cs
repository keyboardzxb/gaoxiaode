// 文件路径: Arrowgene.O2Jam.Server/PacketHandle/ChannelHandle.cs
using Arrowgene.Buffers;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.State;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class ChannelHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ChannelHandle));

        public override PacketId Id => PacketId.ChannelReq;

        public override void Handle(Client client, NetPacket packet)
        {
            IBuffer buffer = packet.CreateReadBuffer();
            ushort selectedPlanet = buffer.ReadUInt16();
            ushort selectedChannel = buffer.ReadUInt16();
            client.CurrentChannelId = selectedChannel;

            IBuffer res = new StreamBuffer();
            res.WriteUInt32(0);
            res.WriteUInt32(1); // Player rank
            client.Send(res.GetAllBytes(), PacketId.ChannelRes);

            // After successfully joining a channel, the server must send the character data.
            // This logic is moved from the (unused) CharacterHandle.
            if (client.Account == null || client.Character == null)
            {
                Logger.Error($"Channel join request successful, but no account/character data in session. Disconnecting client.");
                client.Close();
                return;
            }

            Character character = client.Character;
            IBuffer charRes = new StreamBuffer();

            charRes.WriteInt32(0); // Status Code, 0 for success
            charRes.WriteCString(character.Name);
            charRes.WriteByte((byte)character.Gender);
            charRes.WriteInt32(0); // Unknown
            charRes.WriteInt32(0); // Unknown
            charRes.WriteInt32(character.Gems);
            charRes.WriteInt32(character.Level);
            charRes.WriteInt32(0); // Unknown
            charRes.WriteInt32(0); // Unknown
            charRes.WriteInt32(0); // Unknown
            charRes.WriteInt32(character.Exp);
            charRes.WriteInt32(0); // Unknown
            charRes.WriteByte(0);  // Unknown

            // Equipped Items
            charRes.WriteInt32(character.Instrument);
            charRes.WriteInt32(character.Hat);
            charRes.WriteInt32(character.Props);
            charRes.WriteInt32(character.Glove);
            charRes.WriteInt32(character.Necklace);
            charRes.WriteInt32(character.Top);
            charRes.WriteInt32(character.Bottom);
            charRes.WriteInt32(character.Glasses);
            charRes.WriteInt32(character.Earring);
            charRes.WriteInt32(character.CostumeProps);
            charRes.WriteInt32(character.Shoes);
            charRes.WriteInt32(character.Earring); // Face ID

            // Equipped Items Block 2
            charRes.WriteInt32(character.Wing);
            charRes.WriteInt32(character.InstrumentProps);
            charRes.WriteInt32(character.Pet);
            charRes.WriteInt32(character.HairAccessory);
            charRes.WriteInt32(character.SetAccessory);

            // My Bag (Inventory)
            charRes.WriteInt32(1);
            charRes.WriteInt32(0);
            charRes.WriteInt32(0);

            // Present box
            charRes.WriteInt16(0);

            // Static fields
            charRes.WriteInt16(0);
            charRes.WriteInt32(0);
            charRes.WriteInt32(0);

            // Second Cash Point value
            charRes.WriteInt32(character.Cash);

            // Item rings
            charRes.WriteInt32(0);

            // Penalty info
            charRes.WriteInt16((short)character.PenaltyCount);
            charRes.WriteInt16((short)character.PenaltyLevel);

            client.Send(charRes.GetAllBytes(), PacketId.CharacterRes);
            Logger.Info($"Sent character data for '{character.Name}' after channel join.");


            Lobby.AddClient(client);

            // (修复) 调用正确的方法
            RoomListHandle.SendUnlockPacket(client);
            //RoomListHandle.AnnounceAllRoomsTo(client);
        }
    }
}