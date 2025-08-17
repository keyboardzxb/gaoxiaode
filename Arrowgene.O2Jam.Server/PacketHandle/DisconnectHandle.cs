// Arrowgene.O2Jam.Server/PacketHandle/DisconnectHandle.cs
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.State;
using Arrowgene.O2Jam.Server.Models;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class DisconnectHandle : PacketHandler
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(DisconnectHandle));

        public override PacketId Id => PacketId.DisconnectReq;

        public override void Handle(Client client, NetPacket packet)
        {
            // If the client never successfully logged in, their Account will be null.
            // We only need to perform cleanup if the login was successful.
            if (client.Account != null)
            {
                Logger.Info($"'{client.Account.Username}' initiated disconnect.");

                // Remove client from the channel they are in, if any.
                Channel channel = client.Channel;
                if (channel != null)
                {
                    channel.RemoveClient(client);
                }

                // Remove client from the main lobby list.
                Lobby.RemoveClient(client);
            }
            else
            {
                Logger.Info("A client that had not logged in has disconnected.");
            }

            // The ServerConsumer will handle the actual socket closing after this handler completes.
        }
    }
}