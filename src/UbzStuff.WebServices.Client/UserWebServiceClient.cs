using System;
using System.IO;
using System.ServiceModel;
using UbzStuff.Core.Serialization;
using UbzStuff.Core.Serialization.Views;
using UbzStuff.Core.Views;
using UbzStuff.WebServices.Contracts;

namespace UbzStuff.WebServices.Client
{
    public class UserWebServiceClient : BaseWebServiceClient<IUserWebServiceContract>
    {
        public UserWebServiceClient(string endPoint) : base(endPoint, "UserWebService")
        {
            // Space
        }

        public  UberstrikeUserView GetMember(string authToken)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);

                var data = Channel.GetMember(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return UberstrikeUserViewProxy.Deserialize(inBytes);
            }
        }
    }
}
