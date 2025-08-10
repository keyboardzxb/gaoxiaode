using Arrowgene.O2Jam.Server.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Arrowgene.O2Jam.Server.Packet
{
    // ����һ���µĳ�����࣬������Ҫ�������ݿ�Ĵ�������Ӧ�̳���
    public abstract class DBProvidePacketHandler : PacketHandler
    {
        protected readonly IServiceProvider ServiceProvider;

        protected DBProvidePacketHandler(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }

    // ����ԭʼ�Ĵ���������
    public abstract class PacketHandler : IPacketHandler
    {
        public abstract PacketId Id { get; }
        public abstract void Handle(Client client, NetPacket packet);
    }
}