// 文件路径: Arrowgene.O2Jam.Server/State/Lobby.cs
using Arrowgene.O2Jam.Server.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.O2Jam.Server.State
{
    public enum RoomMode : byte
    {
        SevenKey = 0,
        ThreeKey = 1,
        VsMode = 2
    }

    // 使用 partial class 允许我们在 HttpServer.cs 中对它进行扩展
    public partial class Room
    {
        public int Id { get; }

        public string Password { get; set; }
        public bool IsPlaying { get; set; }
        public int SongId { get; set; }
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
            AddPlayer(host);
        }

        public void AddPlayer(Client player)
        {
            if (Players.TryAdd(player.Account.Id, player))
            {
                player.CurrentRoomId = this.Id;
            }
        }

        public void RemovePlayer(Client player)
        {
            if (Players.TryRemove(player.Account.Id, out _))
            {
                player.CurrentRoomId = -1;
            }
        }
    }


    public static class Lobby
    {
        private static readonly ConcurrentDictionary<int, Client> Clients = new ConcurrentDictionary<int, Client>();
        private static readonly ConcurrentDictionary<int, Room> Rooms = new ConcurrentDictionary<int, Room>();

        public static void AddClient(Client client)
        {
            Clients.TryAdd(client.Account.Id, client);
            client.CurrentRoomId = -1; // 确保新加入的客户端在Lobby
        }

        public static void RemoveClient(Client client)
        {
            Clients.TryRemove(client.Account.Id, out _);
            if (client.CurrentRoomId != -1)
            {
                RemovePlayerFromRoom(client, client.CurrentRoomId);
            }
        }

        public static Room CreateRoom(string name, Client host)
        {
            var newRoom = new Room(name, host)
            {
                // 默认创建的房间是7键模式
                Mode = RoomMode.SevenKey
            };
            Rooms.TryAdd(newRoom.Id, newRoom);
            return newRoom;
        }

        public static List<Room> GetRooms() => Rooms.Values.ToList();
        public static IEnumerable<Client> GetAllClients() => Clients.Values;

        public static Room GetRoom(int roomId)
        {
            Rooms.TryGetValue(roomId, out var room);
            return room;
        }

        public static void RemovePlayerFromRoom(Client player, int roomId)
        {
            if (Rooms.TryGetValue(roomId, out var room))
            {
                room.RemovePlayer(player);
                // 如果房间空了，或者房主走了，就移除房间
                if (room.Players.IsEmpty || room.Host == player)
                {
                    RemoveRoom(roomId);
                }
            }
            player.CurrentRoomId = -1;
        }

        public static void RemoveRoom(int roomId)
        {
            Rooms.TryRemove(roomId, out _);
        }

        public static List<Client> GetLobbyClientsExcept(Client excludeClient)
        {
            return Clients.Values.Where(c => c != excludeClient && c.CurrentRoomId == -1).ToList();
        }

        public static Room GetPlayerRoom(Client player)
        {
            if (player.CurrentRoomId != -1 && Rooms.TryGetValue(player.CurrentRoomId, out var room))
            {
                return room;
            }
            return null;
        }
    }
}