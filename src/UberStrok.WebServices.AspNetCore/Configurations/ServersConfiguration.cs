using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.WebServices.AspNetCore.Configurations
{
    public class ServersConfiguration
    {
        // TODO: Rework this into CommServerConfiguration & GameServerConfiguration.

        public class Server
        {
            public string Host { get; set; }
            public string Name { get; set; }
            public int Port { get; set; }
            public int PhotonId { get; set; }
            public int MinLatency { get; set; }
            public RegionType Region { get; set; }
            public PhotonUsageType UsageType { get; set; }
        };

        public int ResolveInterval { get; set; }
        public Server CommServer { get; set; }
        public List<Server> GameServers { get; set; }
    }
}
