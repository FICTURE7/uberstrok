using log4net;
using System;
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
    public abstract class BaseAuthenticationWebService : BaseWebService, IAuthenticationWebServiceContract
    {
        private readonly static ILog Log = LogManager.GetLogger(typeof(BaseAuthenticationWebService));

        protected BaseAuthenticationWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public abstract AccountCompletionResultView OnCompleteAccount(int cmid, string name, ChannelType channelType, string locale, string machineId);
        public abstract MemberAuthenticationResultView OnLoginSteam(string steamId, string authToken, string machineId);

        byte[] IAuthenticationWebServiceContract.CompleteAccount(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var cmid = Int32Proxy.Deserialize(bytes);
                    var name = StringProxy.Deserialize(bytes);
                    var channelType = EnumProxy<ChannelType>.Deserialize(bytes);
                    var locale = StringProxy.Deserialize(bytes);
                    var machineId = StringProxy.Deserialize(bytes);

                    var view = OnCompleteAccount(cmid, name, channelType, locale, machineId);
                    using (var outBytes = new MemoryStream())
                    {
                        AccountCompletionResultViewProxy.Serialize(outBytes, view);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle CompleteAccount request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.CreateUser(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle CreateUser request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.LinkSteamMember(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LinkSteamMember request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.LoginMemberEmail(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginMemberEmail request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.LoginMemberFacebookUnitySdk(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginMemberFacebookUnitySdk request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.LoginMemberPortal(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginMemberPortal request:");
                Log.Error(ex);
                return null;
            }
        }

        byte[] IAuthenticationWebServiceContract.LoginSteam(byte[] data)
        {
            try
            {
                using (var bytes = new MemoryStream(data))
                {
                    var steamId = StringProxy.Deserialize(bytes);
                    var authToken = StringProxy.Deserialize(bytes);
                    var machineId = StringProxy.Deserialize(bytes);

                    var view = OnLoginSteam(steamId, authToken, machineId);
                    using (var outBytes = new MemoryStream())
                    {
                        MemberAuthenticationResultViewProxy.Serialize(outBytes, view);
                        return outBytes.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle LoginSteam request:");
                Log.Error(ex);
                return null;
            }
        }
    }
}
