using System;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.AspNetCore
{
    public abstract class BaseAuthenticationWebService : IAuthenticationAsyncWebServiceContract
    {
        public abstract Task<AccountCompletionResultView> CompleteAccount(int cmid, string name, ChannelType channelType, string locale, string machineId);
        public abstract Task<MemberAuthenticationResultView> LoginSteam(string steamId, string authToken, string machineId);

        async Task<byte[]> IAuthenticationAsyncWebServiceContract.CompleteAccount(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var cmid = Int32Proxy.Deserialize(bytes);
                var name = StringProxy.Deserialize(bytes);
                var channelType = EnumProxy<ChannelType>.Deserialize(bytes);
                var locale = StringProxy.Deserialize(bytes);
                var machineId = StringProxy.Deserialize(bytes);
                var view = await CompleteAccount(cmid, name, channelType, locale, machineId);
                if (view == null)
                    return null;

                using (var outBytes = new MemoryStream())
                {
                    AccountCompletionResultViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        Task<byte[]> IAuthenticationAsyncWebServiceContract.CreateUser(byte[] data)
        {
            throw new NotImplementedException();
        }

        Task<byte[]> IAuthenticationAsyncWebServiceContract.LinkSteamMember(byte[] data)
        {
            throw new NotImplementedException();
        }

        Task<byte[]> IAuthenticationAsyncWebServiceContract.LoginMemberEmail(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IAuthenticationAsyncWebServiceContract.LoginMemberEmail));

        Task<byte[]> IAuthenticationAsyncWebServiceContract.LoginMemberFacebookUnitySdk(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IAuthenticationAsyncWebServiceContract.LoginMemberFacebookUnitySdk));

        Task<byte[]> IAuthenticationAsyncWebServiceContract.LoginMemberPortal(byte[] data)
            => ThrowHelpers.ThrowOperationNotSupported(nameof(IAuthenticationAsyncWebServiceContract.LoginMemberPortal));

        async Task<byte[]> IAuthenticationAsyncWebServiceContract.LoginSteam(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var steamId = StringProxy.Deserialize(bytes);
                var authToken = StringProxy.Deserialize(bytes);
                var machineId = StringProxy.Deserialize(bytes);
                var view = await LoginSteam(steamId, authToken, machineId);
                if (view == null)
                    return null;

                using (var outBytes = new MemoryStream())
                {
                    MemberAuthenticationResultViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }
    }
}
