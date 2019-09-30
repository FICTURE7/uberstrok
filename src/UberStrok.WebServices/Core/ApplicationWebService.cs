using log4net;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.Core
{
    public class ApplicationWebService : BaseApplicationWebService
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(ApplicationWebService).Name);

        public ApplicationWebService(WebServiceContext ctx) : base(ctx)
        {
            var config = Utils.DeserializeJsonAt<ApplicationConfigurationView>("configs/game/application.json");
            if (config == null)
                throw new FileNotFoundException("configs/game/application.json file not found.");

            _appConfig = config;
        }

        private ApplicationConfigurationView _appConfig;

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
                WarnPlayer = true,

                // Let the client know where the Comm servers and game servers are.
                CommServer = Context.Servers.CommServer,
                GameServers = Context.Servers.GameServers
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

            return Context.Maps.GetAll();
        }
    }
}
