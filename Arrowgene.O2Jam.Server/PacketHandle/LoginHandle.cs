// Arrowgene.O2Jam.Server/PacketHandle/LoginHandle.cs
using Arrowgene.Buffers;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Data;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.Logging;
using System;

namespace Arrowgene.O2Jam.Server.PacketHandle
{
    public class LoginHandle : PacketHandler
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(LoginHandle));

        public override PacketId Id => PacketId.LoginReq;

        public override void Handle(Client client, NetPacket packet)
        {
            IBuffer buffer = packet.CreateReadBuffer();
            buffer.Position = 25;
            string username = buffer.ReadCString();
            string password = buffer.ReadCString();

            Logger.Info($"Login attempt from game client: User='{username}'");

            Account account = DatabaseManager.GetAccount(username, password);
            if (account == null)
            {
                Logger.Info($"Login failed for user: '{username}'. Invalid credentials.");
                IBuffer failureRes = new StreamBuffer();
                failureRes.WriteInt32(1);
                client.Send(failureRes.GetAllBytes(), PacketId.LoginRes);
                return;
            }

            // ***** �����Ե����� - ��ʼ *****

            // �ڳɹ���֤�˻������Ǳ������̼�������˻������Ľ�ɫ���ݡ�
            Character character = DatabaseManager.GetCharacterByAccountId(account.Id);
            if (character == null)
            {
                // ����һ�����ش�����ζ�����ݿ��д����˻���û�ж�Ӧ�Ľ�ɫ��
                Logger.Error($"FATAL: Character data not found for account '{username}' (Id: {account.Id}). Disconnecting.");
                client.Close(); // ����Ҳ�����ɫ����Ͽ����ӡ�
                return;
            }

            Logger.Info($"Successfully loaded User '{username}' and Character '{character.Name}'.");

            // ���˻��ͽ�ɫ��Ϣ�������ڿͻ��˵ĻỰ״̬�У��Թ������� CharacterHandle ʹ�á�
            client.Account = account;
            client.Character = character;

            // ***** �����Ե����� - ���� *****


            // ���������͵�¼�ɹ��Ļ�Ӧ�� (�ⲿ���߼����ֲ���)
            IBuffer successRes = new StreamBuffer();
            successRes.WriteInt32(0);
            successRes.WriteByte(32);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteCString(DateTime.Now.ToString("yyyy-dd-MM hh:mm:ss"));
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteUInt16(0);
            successRes.WriteByte(1);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteCString("Test2");
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteByte(255);
            successRes.WriteUInt16(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            successRes.WriteByte(0);
            client.Send(successRes.GetAllBytes(), PacketId.LoginRes);
        }
    }
}
