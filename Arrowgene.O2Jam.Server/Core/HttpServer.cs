// Arrowgene.O2Jam.Server/Core/HttpServer.cs
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Logging;
using Arrowgene.O2Jam.Server.State;
using System.Net;
using System.Text;
using System.Threading;
using System.Linq; // 需要引入Linq
using System;
using System.Collections.Generic; // 需要引入Collections.Generic
using Arrowgene.O2Jam.Server.Models;

namespace Arrowgene.O2Jam.Server.Core
{
    public class HttpServer
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(HttpServer));
        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;
        private static readonly Encoding KoreanEncoding = Encoding.GetEncoding("EUC-KR", new EncoderReplacementFallback("?"), new DecoderReplacementFallback("?"));

        public HttpServer(string prefix)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
            _listenerThread = new Thread(HandleRequests);
        }

        public void Start()
        {
            _listener.Start();
            _listenerThread.Start();
            Logger.Info($"HTTP Server started. Listening on {_listener.Prefixes.FirstOrDefault()}");
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        private void HandleRequests()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var context = _listener.GetContext();
                    ProcessRequest(context);
                }
                catch (HttpListenerException) { break; }
                catch (Exception ex) { Logger.Error($"Unhandled exception in HTTP request handler: {ex}"); }
            }
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            string responseString = "";

            Logger.Info($"HTTP Request received: {request.Url.AbsoluteUri}");

            switch (request.Url.AbsolutePath.ToLower())
            {
                case "/gamefind/gamefine_main.asp":
                    responseString = HandleGameFindMain(request);
                    break;
                case "/gamefind/gamefind_user_list.asp":
                    responseString = HandleUserList(request);
                    break;
                case "/gamefind/friend_list.asp":
                    responseString = HandleFriendList(request);
                    break;
                default:
                    responseString = ""; // 默认返回空内容
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
            }

            byte[] buffer = KoreanEncoding.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        // --- 核心修正：房间列表处理 ---
        private string HandleGameFindMain(HttpListenerRequest request)
        {
            var rooms = Lobby.GetRooms();

            // 【修正点 1】当没有房间时，必须返回一个完全空的字符串，而不是 "start"
            if (!rooms.Any())
            {
                return "";
            }

            var roomStrings = new List<string>();
            foreach (var room in rooms)
            {
                // 格式: roomId&roomName&password&maxPlayers&currentPlayers&songId&status&isVs&hostName
                string roomData = string.Join("&",
                    room.Id,
                    room.Name,
                    !string.IsNullOrEmpty(room.Password) ? 1 : 0,
                    8, // Max Players
                    room.Players.Count,
                    room.IsPlaying ? room.SongId : 0,
                    room.IsPlaying ? 2 : 1, // 1: Waiting, 2: Playing
                    (int)room.Mode,
                    room.Host.Character.Name
                );
                roomStrings.Add(roomData);
            }

            // 【修正点 2】使用 string.Join 来确保最后一个房间后面没有多余的 "|" 分隔符
            return string.Join("|", roomStrings);
        }

        // --- 核心修正：玩家列表处理 ---
        private string HandleUserList(HttpListenerRequest request)
        {
            var clients = Lobby.GetAllClients();

            // 【修正点 1】当没有玩家时，同样返回空字符串
            if (!clients.Any())
            {
                return "";
            }

            var userStrings = new List<string>();
            foreach (var client in clients)
            {
                // 格式: userId,userName,level,avatarId,location
                string userData = string.Join(",",
                   client.Account.Id,
                   client.Character.Name,
                   client.Character.Level,
                   0, // Avatar ID, 暂时硬编码为0
                   client.CurrentRoomId == -1 ? "0" : client.CurrentRoomId.ToString() // 0=Lobby, >0=Room ID
                );
                userStrings.Add(userData);
            }

            // 【修正点 2】使用 string.Join 确保最后一个玩家信息后没有多余的 "|"
            return string.Join("|", userStrings);
        }

        // --- 核心修正：好友列表处理 ---
        private string HandleFriendList(HttpListenerRequest request)
        {
            // 对于好友列表，最安全的默认返回值也是空字符串。
            return "";
        }
    }

    // 这部分代码应该已经在您的Lobby.cs中了，这里只是为了完整性
    // public partial class Room
    // {
    //     public string Password { get; set; } = null;
    //     public bool IsPlaying { get; set; } = false;
    //     public int SongId { get; set; } = 0;
    // }
}