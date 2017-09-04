using System.IO;
using UbzStuff.Core.Common;
using UbzStuff.Core.Serialization;
using UbzStuff.Core.Serialization.Views;
using UbzStuff.Core.Views;
using UbzStuff.WebServices.Contracts;

namespace UbzStuff.WebServices.Client
{
    public class AuthenticationWebServiceClient : BaseWebServiceClient<IAuthenticationWebServiceContract>
    {
        public AuthenticationWebServiceClient(string endPoint) : base(endPoint, "AuthenticationWebService")
        {
            // Space
        }

        public AccountCompletionResultView CompleteAccount(int cmid, string name, ChannelType channelType, string locale, string machineId)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                EnumProxy<ChannelType>.Serialize(bytes, channelType);
                StringProxy.Serialize(bytes, locale);
                StringProxy.Serialize(bytes, machineId);

                var data = Channel.CompleteAccount(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return AccountCompletionResultViewProxy.Deserialize(inBytes);
            }
        }

        public MemberAuthenticationResultView LoginSteam(string steamId, string authToken, string machineId)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, steamId);
                StringProxy.Serialize(bytes, authToken);
                StringProxy.Serialize(bytes, machineId);

                var data = Channel.LoginSteam(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return MemberAuthenticationResultViewProxy.Deserialize(inBytes);
            }
        }
    }
}
