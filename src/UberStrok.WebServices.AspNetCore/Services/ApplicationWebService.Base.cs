using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.AspNetCore
{
    public abstract class BaseApplicationWebService : IApplicationAsyncWebServiceContract
    {
        protected abstract Task<AuthenticateApplicationView> AuthenticateApplication(string clientVersion, ChannelType channelType, string publicKey);
        protected abstract Task<ApplicationConfigurationView> GetConfigurationData(string clientVersion);
        protected abstract Task<List<MapView>> GetMaps(string clientVersion, DefinitionType definitionType);

        async Task<byte[]> IApplicationAsyncWebServiceContract.AuthenticateApplication(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var version = StringProxy.Deserialize(bytes); // 4.7.1
                var channelType = EnumProxy<ChannelType>.Deserialize(bytes); // Steam
                var publicKey = StringProxy.Deserialize(bytes); // string.Empty;
                var view = await AuthenticateApplication(version, channelType, publicKey);
                if (view == null)
                    return null;

                using (var outBytes = new MemoryStream())
                {
                    AuthenticateApplicationViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IApplicationAsyncWebServiceContract.GetConfigurationData(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var version = StringProxy.Deserialize(bytes);
                var view = await GetConfigurationData(version);
                if (view == null)
                    return null;

                using (var outBytes = new MemoryStream())
                {
                    ApplicationConfigurationViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IApplicationAsyncWebServiceContract.GetMaps(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var version = StringProxy.Deserialize(bytes);
                var definitionType = EnumProxy<DefinitionType>.Deserialize(bytes);
                var view = await GetMaps(version, definitionType);
                if (view == null)
                    return null;

                using (var outBytes = new MemoryStream())
                {
                    ListProxy<MapView>.Serialize(outBytes, view, MapViewProxy.Serialize);
                    return outBytes.ToArray();
                }
            }
        }

        Task<byte[]> IApplicationAsyncWebServiceContract.SetMatchScore(byte[] data)
        {
            throw new System.NotImplementedException();
        }
    }
}
