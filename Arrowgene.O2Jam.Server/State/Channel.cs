using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Arrowgene.O2Jam.Server.Core;
using Arrowgene.O2Jam.Server.Packet;
using Arrowgene.O2Jam.Server.Models;




namespace Arrowgene.O2Jam.Server.State
{
    public class Channel
    {
        public ConcurrentDictionary<int, Room> Rooms { get; }
        public ConcurrentDictionary<int, Client> Clients { get; }

        public Channel()
        {
            Rooms = new ConcurrentDictionary<int, Room>();
            Clients = new ConcurrentDictionary<int, Client>();
        }

        public void AddClient(Client client)
        {
            Clients.TryAdd(client.Account.Id, client); // 修改为使用 Account.Id  
        }

        public void RemoveClient(Client client)
        {
            Clients.TryRemove(client.Account.Id, out _); // 修改为使用 Account.Id  

            // 如果玩家在某个房间里，也从房间中移除  
            foreach (var room in Rooms.Values)
            {
                room.RemovePlayer(client);
            }
        }

        public Room CreateRoom(string name, Client creator)
        {
            var room = new Room(name, creator);
            room.AddPlayer(creator);
            Rooms.TryAdd(room.Id, room);
            return room;
        }

        public void RemoveRoom(int roomId)
        {
            Rooms.TryRemove(roomId, out _);
        }

        public Room GetRoom(int roomId)
        {
            Rooms.TryGetValue(roomId, out var room);
            return room;
        }

        public List<Room> GetRoomList()
        {
            return Rooms.Values.ToList();
        }

        // 用于向频道内所有玩家广播消息  
        public void Broadcast(byte[] packet, PacketId packetId) // 修改参数类型为 PacketId  
        {
            foreach (var client in Clients.Values)
            {
                client.Send(packet, packetId);
            }
        }
    }
}