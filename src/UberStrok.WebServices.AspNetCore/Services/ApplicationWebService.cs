using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Configurations;

namespace UberStrok.WebServices.AspNetCore
{
    public class ApplicationWebService : BaseApplicationWebService
    {
        private class Events
        {
            public static readonly EventId Resolve = new EventId(1000, nameof(Resolve));
        }

        private DateTime _nextResolve;

        private PhotonView _commServer;
        private List<PhotonView> _gameServers;

        private readonly ILogger<ApplicationWebService> _logger;
        private readonly ServersConfiguration _serversConfig;
        private readonly ApplicationConfiguration _appConfig;

        // Cached responses.
        private readonly Task<ApplicationConfigurationView> _GetConfigurationDataCache;
        private readonly Task<List<MapView>> _GetMapsCache;

        public ApplicationWebService(
            ILogger<ApplicationWebService> logger, 
            IOptions<ServersConfiguration> serversConfig, 
            IOptions<ApplicationConfiguration> appConfig, 
            MapManager maps)
        {
            _logger = logger;

            _appConfig = appConfig.Value;
            _serversConfig = serversConfig.Value;

            // TODO: Refactor resolving code.
            // TODO: Handle serversConfig.Servers being null.

            _GetConfigurationDataCache = Task.FromResult<ApplicationConfigurationView>(_appConfig);
            _GetMapsCache = Task.FromResult(maps.GetView());
        }

        protected override async Task<AuthenticateApplicationView> AuthenticateApplication(string clientVersion, ChannelType channelType, string publicKey)
        {
            var result = new AuthenticateApplicationView
            {
                EncryptionInitVector = default, // Not used.
                EncryptionPassPhrase = default // Not used.
            };

            // If client not same version as configured, disable and warn 
            // client.
            if (clientVersion != _appConfig.Version)
            {
                result.IsEnabled = false;
                result.WarnPlayer = true;
            }
            else
            {
                if (DateTime.UtcNow >= _nextResolve)
                    await ResolveAllServers(_serversConfig);

                result.IsEnabled = true;
                result.WarnPlayer = false;

                // Let the client know where the Comm server and Game servers are.
                result.CommServer = _commServer;
                result.GameServers = _gameServers;
            }

            return result;
        }

        protected override Task<ApplicationConfigurationView> GetConfigurationData(string clientVersion)
            => clientVersion == _appConfig.Version ? _GetConfigurationDataCache : null;

        protected override Task<List<MapView>> GetMaps(string clientVersion, DefinitionType definitionType)
            => clientVersion == _appConfig.Version ? _GetMapsCache : null;

        private async Task ResolveAllServers(ServersConfiguration config)
        {
            var commServerTask = ResolveServer(config.CommServer);
            var gameServerTasks = new List<Task<PhotonView>>(config.GameServers.Count);
            foreach (var server in config.GameServers)
                gameServerTasks.Add(ResolveServer(server));

            // Do as much work in parrallel as possible.
            await Task.WhenAll(commServerTask, Task.WhenAll(gameServerTasks));

            _commServer = commServerTask.Result;
            _commServer.UsageType = PhotonUsageType.CommServer;
            _gameServers = new List<PhotonView>(gameServerTasks.Count);
            foreach (var task in gameServerTasks)
            {
                if (task.Result != null)
                    _gameServers.Add(task.Result);
            }

            // Resolve hosts at least ResolveInterval seconds, or 10 if its
            // less or equal to 0.
            _nextResolve = DateTime.UtcNow.AddSeconds(_serversConfig.ResolveInterval <= 0 ? 10 : _serversConfig.ResolveInterval);
        }

        private async Task<PhotonView> ResolveServer(ServersConfiguration.Server config)
        {
            var ip = default(string);

            // Try to parse address and
            if (IPAddress.TryParse(config.Host, out IPAddress addr))
            {
                ip = addr.ToString();
            }
            else
            {
                _logger.LogDebug(Events.Resolve, "Resolving an address for \"{host}\"...", config.Host);

                // Attempt to catch exception so we can log it afterwards if it
                // is ever thrown.
                var exception = default(Exception);

                try
                {
                    var addresses = await Dns.GetHostAddressesAsync(config.Host);

                    // Find first IPv4 address in the array of addresses resolved.
                    // XXX: Could be improved by mapping IPv6 to IPv4?
                    foreach (var otherAddr in addresses)
                    {
                        if (otherAddr.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ip = otherAddr.ToString();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                    // Defensive set.
                    ip = null;
                }

                // If we could not resolve the address of the host.
                if (ip == null)
                {
                    // TODO: Check if ILogger handles null exceptions. 
                    if (exception == null)
                        _logger.LogError(Events.Resolve, "Unable to resolve \"{host}\".", config.Host);
                    else
                        _logger.LogError(Events.Resolve, exception, "Unable to resolve \"{host}\".", config.Host);
                    return null;
                }
            }

            _logger.LogDebug(Events.Resolve, "Resolved \"{host}\" to \"{ip}\"...", config.Host, ip);

            return new PhotonView
            {
                IP = ip,
                MinLatency = config.MinLatency,
                Name = config.Name,
                PhotonId = config.PhotonId,
                Port = config.Port,
                Region = config.Region,
                UsageType = config.UsageType
            };
        }
    }
}
