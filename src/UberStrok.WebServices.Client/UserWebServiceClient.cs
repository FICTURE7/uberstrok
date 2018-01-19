using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Client
{
    public class UserWebServiceClient : BaseWebServiceClient<IUserWebServiceContract>
    {
        public UserWebServiceClient(string endPoint) : base(endPoint, "UserWebService")
        {
            // Space
        }

        public UberstrikeUserView GetMember(string authToken)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);

                var data = Channel.GetMember(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return UberstrikeUserViewProxy.Deserialize(inBytes);
            }
        }

        public LoadoutView GetLoadout(string authToken)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);

                var data = Channel.GetLoadout(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return LoadoutViewProxy.Deserialize(inBytes);
            }
        }
    }
}
