using System;
using System.IO;
using System.ServiceModel;
using UbzStuff.Core.Serialization;
using UbzStuff.Core.Serialization.Views;
using UbzStuff.Core.Views;
using UbzStuff.WebServices.Contracts;

namespace UbzStuff.WebServices.Client
{
    public class UserWebServiceClient
    {
        public UserWebServiceClient(string webServer)
        {
            if (webServer == null)
                throw new ArgumentNullException(nameof(webServer));

            var builder = new UriBuilder(webServer);
            builder.Path = Path.Combine(builder.Path, "UserWebService");

            var endPoint = new EndpointAddress(builder.Uri);

            var binding = new BasicHttpBinding();
            var factory = new ChannelFactory<IUserWebServiceContract>(binding, endPoint);

            _service = factory.CreateChannel();
        }

        private readonly IUserWebServiceContract _service;

        public  UberstrikeUserView GetMember(string authToken)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);

                var data = _service.GetMember(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                {
                    var member = UberstrikeUserViewProxy.Deserialize(inBytes);
                    return member;
                }
            }
        }
    }
}
