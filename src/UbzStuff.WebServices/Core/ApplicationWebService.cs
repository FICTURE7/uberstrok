using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UbzStuff.Core.Common;
using UbzStuff.Core.Views;

namespace UbzStuff.WebServices.Core
{
    public class ApplicationWebService : BaseApplicationWebService
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(ApplicationWebService));

        public ApplicationWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        private PhotonView _commServer;
        private List<PhotonView> _gameServers;

        private List<MapView> _maps;
        private ApplicationConfigurationView _appConfig;

        protected internal override bool SetUp()
        {
            var mapsJson = File.ReadAllText("configs/maps.json");
            var appConfigJson = File.ReadAllText("configs/application.json");
            var serversJson = File.ReadAllText("configs/servers.json");

            _maps = JsonConvert.DeserializeObject<List<MapView>>(mapsJson);
            _appConfig = JsonConvert.DeserializeObject<ApplicationConfigurationView>(appConfigJson);

            try
            {
                var id = 0;
                var servers = JObject.Parse(serversJson);
                var commServer = servers["CommServer"];
                var gameServers = servers["GameServers"];

                _gameServers = new List<PhotonView>();
                _commServer = new PhotonView
                {
                    PhotonId = ++id,
                    IP = commServer["IP"].ToObject<string>(),
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
                        IP = token["IP"].ToObject<string>(),
                        Port = token["Port"].ToObject<int>(),
                        Region = token["Region"].ToObject<RegionType>(),
                        UsageType = PhotonUsageType.All,
                        Name = token["Name"].ToObject<string>(),
                        MinLatency = token["MinLatency"].ToObject<int>()
                    };

                    _gameServers.Add(server);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to parse configs/servers.json:");
                Log.Error(ex);
                return false;
            }

            return true;
        }

        protected override AuthenticateApplicationView OnAuthenticateApplication(string clientVersion, ChannelType channelType, string publicKey)
        {
            Log.Info($"Authenticating client v{clientVersion} -> {channelType}.");

            // Check if they running on the appropriate version and channel.
            if (clientVersion != "4.7.1" || channelType != ChannelType.Steam)
                return null;

            var view = new AuthenticateApplicationView
            {
                EncryptionInitVector = string.Empty, // Not used by the UberStrike client?
                EncryptionPassPhrase = string.Empty, // Same?

                IsEnabled = true,
                WarnPlayer = false,

                CommServer = _commServer,
                GameServers = _gameServers
            };
            return view;
        }

        protected override ApplicationConfigurationView OnGetConfigurationData(string clientVersion)
        {
            // Check if they running on the appropriate version.
            if (clientVersion != "4.7.1")
                return null;

            return _appConfig;
        }

        protected override List<MapView> OnGetMaps(string clientVersion, DefinitionType definitionType)
        {
            // Check if they running on the appropriate version and definitionType.
            if (clientVersion != "4.7.1")
                return null;

            return _maps;
        }
    }
}
