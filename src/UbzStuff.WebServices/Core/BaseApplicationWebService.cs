using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using UbzStuff.Core.Serialization;
using UbzStuff.Core.Serialization.Views;
using UbzStuff.Core.Common;
using UbzStuff.Core.Views;
using UbzStuff.WebServices.Contracts;

namespace UbzStuff.WebServices.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public abstract class BaseApplicationWebService : BaseWebService, IApplicationWebServiceContract
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(BaseApplicationWebService));

        protected BaseApplicationWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        protected abstract AuthenticateApplicationView OnAuthenticateApplication(string clientVersion, ChannelType channelType, string publicKey);
        protected abstract ApplicationConfigurationView OnGetConfigurationData(string clientVersion);
        protected abstract List<MapView> OnGetMaps(string clientVersion, DefinitionType definitionType);

        byte[] IApplicationWebServiceContract.AuthenticateApplication(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var version = StringProxy.Deserialize(bytes); // 4.7.1
                    var channelType = EnumProxy<ChannelType>.Deserialize(bytes); // Steam
                    var publicKey = StringProxy.Deserialize(bytes); // string.Empty;

                    var view = OnAuthenticateApplication(version, channelType, publicKey);
                    using (var outBytes = new MemoryStream())
                    {
                        AuthenticateApplicationViewProxy.Serialize(outBytes, view);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle AuthenticateApplication request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IApplicationWebServiceContract.GetConfigurationData(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var version = StringProxy.Deserialize(bytes);

                    var view = OnGetConfigurationData(version);
                    using (var outBytes = new MemoryStream())
                    {
                        ApplicationConfigurationViewProxy.Serialize(outBytes, view);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetConfigurationData request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IApplicationWebServiceContract.GetMaps(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var version = StringProxy.Deserialize(bytes);
                    var definitionType = EnumProxy<DefinitionType>.Deserialize(bytes);

                    var view = OnGetMaps(version, definitionType);
                    using (var outBytes = new MemoryStream())
                    {
                        ListProxy<MapView>.Serialize(outBytes, view, MapViewProxy.Serialize);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMaps request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IApplicationWebServiceContract.SetMatchScore(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetMaps request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
