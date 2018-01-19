using System;
using System.IO;
using System.ServiceModel;

namespace UberStrok.WebServices.Client
{
    public abstract class BaseWebServiceClient<TChannel>
    {
        private readonly TChannel _channel;

        private static readonly BasicHttpBinding s_binding = new BasicHttpBinding();
        private static readonly ChannelFactory<TChannel> s_factory = new ChannelFactory<TChannel>(s_binding);

        protected BaseWebServiceClient(string endPoint, string service)
        {
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));

            var builder = new UriBuilder(endPoint);
            builder.Path = Path.Combine(builder.Path, service);

            var address = new EndpointAddress(builder.Uri);
            _channel = s_factory.CreateChannel(address);
        }

        protected TChannel Channel => _channel;
    }
}
