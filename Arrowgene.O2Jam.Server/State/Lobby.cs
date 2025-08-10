// 文件路径: Arrowgene.O2Jam.Server/State/Lobby.cs

using Arrowgene.O2Jam.Server.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.O2Jam.Server.State
{
    // 定义房间模式的枚举
    public enum RoomMode : byte
    {
        SevenKey = 0, // 7K 模式
        ThreeKey = 1, // 3K 模式
        VsMode = 2  // VS 对战模式
    }

    // 定义一个完整的、包含所有必需方法的 Room 类
    public class Room
    {
        public int Id { get; }
        public string Name { get; set; }
        public RoomMode Mode { get; set; }
        public Client Host { get; }
        public ConcurrentDictionary<int, Client> Players { get; }

        private static int _nextId = 0;

        public Room(string name, Client host)
        {
            Id = System.Threading.Interlocked.Increment(ref _nextId);
            Name = name;
            Host = host;
            Players = new ConcurrentDictionary<int, Client>();
            AddPlayer(host); // 创建房间时，房主自动加入
        }

        // 添加玩家到房间的方法 (修复 CS1061)
        public void AddPlayer(Client player)
        {
            Players.TryAdd(player.Account.Id, player);
        }

        // 从房间移除玩家的方法 (修复 CS1061)
        public void RemovePlayer(Client player)
        {
            Players.TryRemove(player.Account.Id, out _);
        }
    }

    // 全局静态大厅类，用于管理所有房间和玩家
    public static class Lobby
    {
        private static readonly ConcurrentDictionary<int, Room> Rooms = new ConcurrentDictionary<int, Room>();
        private static readonly ConcurrentDictionary<int, Client> Clients = new ConcurrentDictionary<int, Client>();

        public static void AddClient(Client client) => Clients.TryAdd(client.Account.Id, client);
        public static void RemoveClient(Client client)
        {
            Clients.TryRemove(client.Account.Id, out _);
            RemovePlayerFromCurrentRoom(client); // 玩家下线时也确保从房间移除
        }
        public static Room CreateRoom(string name, Client host)
        {
            var newRoom = new Room(name, host);
            Rooms.TryAdd(newRoom.Id, newRoom);
            return newRoom;
        }
        public static List<Room> GetRooms() => Rooms.Values.ToList();
        public static IEnumerable<Client> GetAllClients() => Clients.Values;

        // (新增) 将一个玩家从他所在的房间移除
        public static void RemovePlayerFromCurrentRoom(Client player)
        {
            var room = Rooms.Values.FirstOrDefault(r => r.Players.ContainsKey(player.Account.Id));
            if (room != null)
            {
                room.RemovePlayer(player);
                if (room.Players.IsEmpty || room.Host == player)
                {
                    Rooms.TryRemove(room.Id, out _);
                }
            }
            player.CurrentRoomId = -1;
        }
    


        // 获取某个特定玩家所在的房间
        public static Room GetPlayerRoom(Client player)
        {
            return Rooms.Values.FirstOrDefault(r => r.Players.ContainsKey(player.Account.Id));
        }

        // 获取除指定玩家外的所有大厅玩家
        public static List<Client> GetOtherClients(Client client)
        {
            return Clients.Values.Where(c => c.Account.Id != client.Account.Id).ToList();
        }
    }
}