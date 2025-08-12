// 文件路径: Arrowgene.O2Jam.Server/State/Lobby.cs

using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.PacketHandle;
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
            client.CurrentRoomId = -1;
        }

        public static void RemoveClient(Client client)
        {
            Clients.TryRemove(client.Account.Id, out _);
            // 这里我们不再调用 RemovePlayerFromCurrentRoom，因为下线逻辑会在 Socket 断开时统一处理
        }

        public static Room CreateRoom(string name, Client host)
        {
            var newRoom = new Room(name, host);
            Rooms.TryAdd(newRoom.Id, newRoom);
            return newRoom;
        }

        public static List<Room> GetRooms() => Rooms.Values.ToList();
        public static IEnumerable<Client> GetAllClients() => Clients.Values;

        // 【核心修正】恢复 GetRoom 方法，这是编译错误的原因
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
            }
            else
            {
                player.CurrentRoomId = -1;
            }
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