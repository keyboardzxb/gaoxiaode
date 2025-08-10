// Arrowgene.O2Jam.Server/Core/NetServer.cs (修改后)
using System.Net;
using Arrowgene.O2Jam.Server.PacketHandle;
using Arrowgene.Logging;
using Arrowgene.Networking.Tcp.Server.AsyncEvent;
using Arrowgene.O2Jam.Server.Data;
using Arrowgene.O2Jam.Server.State; // 新增：引入 State 命名空间
using System.Collections.Generic;

namespace Arrowgene.O2Jam.Server.Core
{
    public class NetServer
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(NetServer));

        private readonly AsyncEventServer _server;
        private readonly ServerConsumer _consumer;
        private readonly O2JamDbContext _dbContext;


        public Channel MainChannel { get; } // 新增：公开的频道实例

        public NetServer(Setting setting, O2JamDbContext dbContext)
        {
            Setting = new Setting(setting);
            _dbContext = dbContext;
            MainChannel = new Channel(); // 新增：初始化频道

            // 将 MainChannel 实例传递给需要它的 Handler
            _consumer = new ServerConsumer(Setting.ServerSetting);

            // 注意：构造函数调用已更改，以传递对 MainChannel 的引用
            _consumer.AddHandler(new UnknownHandle());
            _consumer.AddHandler(new LoginHandle());
            _consumer.AddHandler(new RegisterHandle());
            _consumer.AddHandler(new PlanetHandle());
            _consumer.AddHandler(new ChannelHandle()); // 修改
            _consumer.AddHandler(new MusicListHandle(this));
            _consumer.AddHandler(new CharacterHandle());
            _consumer.AddHandler(new RoomListHandle()); // 修改
            _consumer.AddHandler(new CreateRoomHandle()); // 修改
            _consumer.AddHandler(new RoomSongSelectHandle());
            _consumer.AddHandler(new RoomColorSelectHandle());
            _consumer.AddHandler(new RoomApplySkill());
            _consumer.AddHandler(new RoomUnknown2Handle());
            _consumer.AddHandler(new StartGameHandle());
            _consumer.AddHandler(new GameCheck1Handle());
            _consumer.AddHandler(new GameCheck2Handle());
            _consumer.AddHandler(new CashHandle());
            _consumer.AddHandler(new Room1Handle());
            _consumer.AddHandler(new PingHandle());
            _consumer.AddHandler(new DisconnectHandle(MainChannel)); // 修改
            _consumer.AddHandler(new LobbyBackButton());
            _consumer.AddHandler(new RoomBackButtonHandle());
            _consumer.AddHandler(new InGameBackButton());
            _consumer.AddHandler(new RoomSongSelectButton1());
            _consumer.AddHandler(new RoomSongSelectButton2());
            _consumer.AddHandler(new RoomSongSelectCheckButton());
            _consumer.AddHandler(new InGameRanking());
            _consumer.AddHandler(new Resalt());
            _consumer.AddHandler(new LobbyChatHandle(this));
            _consumer.AddHandler(new test());
            _consumer.AddHandler(new test1());
            _consumer.AddHandler(new test2());
            _consumer.AddHandler(new test3());

            _server = new AsyncEventServer(
                IPAddress.Any,
                15010,
                _consumer,
                Setting.ServerSetting
            );
        }



        public Setting Setting { get; }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }

        public List<Client> GetAllClients()
        {
            return _consumer.GetAllClients();
        }
    }
}