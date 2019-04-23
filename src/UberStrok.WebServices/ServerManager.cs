using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.WebServices
{
    /**
     *  Manages Comm servers and game servers.
     */
    
    public class ServerManager
    {

        public ServerManager(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;

            var id = 0;
            var servers = Utils.DeserializeJsonAt<JObject>("configs/game/servers.json");
            if (servers == null)
                throw new FileNotFoundException("configs/game/servers.json file not found.");

            var commServer = servers["CommServer"];
            var gameServers = servers["GameServers"];

            _gameServers = new List<PhotonView>();
            _commServer = new PhotonView
            {
                PhotonId = ++id,
                IP = GetIPAddress(commServer["IP"].ToObject<string>()),
                Port = commServer["Port"].ToObject<int>(),

                Region = RegionType.UsEast,
                UsageType = PhotonUsageType.CommServer,
                Name = "UbzStuff.Realtime.CommServer",
                MinLatency = 0
            };

            foreach (var token in gameServers)
            {
                var server = new PhotonView
                {
                    PhotonId = ++id,
                    IP = GetIPAddress(token["IP"].ToObject<string>()),
                    Port = token["Port"].ToObject<int>(),
                    Region = token["Region"].ToObject<RegionType>(),
                    UsageType = PhotonUsageType.All,
                    Name = token["Name"].ToObject<string>(),
                    MinLatency = token["MinLatency"].ToObject<int>()
                };

                _gameServers.Add(server);
            }
        }

        public PhotonView CommServer => _commServer;
        public List<PhotonView> GameServers => _gameServers;

        private readonly PhotonView _commServer;
        private readonly List<PhotonView> _gameServers;

        private readonly WebServiceContext _ctx;

        public static string GetIPAddress(string hostname)
        {
            IPHostEntry host = Dns.GetHostEntry(hostname);

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }
    }
}
