// Arrowgene.O2Jam.Server/Core/HttpRegistrationServer.cs
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Data;

namespace Arrowgene.O2Jam.Server.Core
{
    public class HttpRegistrationServer
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(HttpRegistrationServer));
        private readonly HttpListener _listener;

        public HttpRegistrationServer(string host = "http://*:15011/")
        {
            if (!HttpListener.IsSupported) throw new NotSupportedException("HttpListener is not supported.");
            _listener = new HttpListener();
            _listener.Prefixes.Add(host);
        }

        public void Start() => Task.Run(() => RunServer());
        public void Stop() => _listener.Stop();

        private async Task RunServer()
        {
            try
            {
                _listener.Start();
                Logger.Info($"HTTP Registration Server listening on http://localhost:15011/");
            }
            catch (HttpListenerException ex)
            {
                Logger.Error($"FATAL: HTTP Server failed to start: {ex.Message}");
                Logger.Error("Please run the following command in an Administrator Command Prompt:");
                Logger.Error("netsh http add urlacl url=http://*:15011/ user=Everyone");
                return;
            }

            while (_listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = await _listener.GetContextAsync();
                    await ProcessRequestAsync(context);
                }
                catch { break; }
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            string responseString = "ERROR_UNKNOWN";

            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/register")
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string body = await reader.ReadToEndAsync();
                    var data = System.Web.HttpUtility.ParseQueryString(body);
                    string username = data["username"];
                    string password = data["password"];

                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        bool success = DatabaseManager.RegisterAccount(username, password);
                        responseString = success ? "SUCCESS" : "ERROR_USER_EXISTS";
                    }
                    else { responseString = "ERROR_INVALID_DATA"; }
                }
            }
            else { response.StatusCode = (int)HttpStatusCode.NotFound; }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}