using Arrowgene.O2Jam.Server.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Arrowgene.O2Jam.Server.Packet
{
    // 这是一个新的抽象基类，所有需要访问数据库的处理器都应继承它
    public abstract class DBProvidePacketHandler : PacketHandler
    {
        protected readonly IServiceProvider ServiceProvider;

        protected DBProvidePacketHandler(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }

    // 这是原始的处理器基类
    public abstract class PacketHandler : IPacketHandler
    {
        public abstract PacketId Id { get; }
        public abstract void Handle(Client client, NetPacket packet);
    }
}