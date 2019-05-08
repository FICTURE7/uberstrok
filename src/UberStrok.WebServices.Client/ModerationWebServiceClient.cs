using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Client
{
    public class ModerationWebServiceClient : BaseWebServiceClient<IModerationWebServiceContract>
    {
        public ModerationWebServiceClient(string endPoint) : base(endPoint, "ModerationWebService")
        {
            // Space
        }

        public int UnbanCmid(string authToken, int cmid)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);
                Int32Proxy.Serialize(bytes, cmid);

                var data = Channel.UnbanCmid(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return Int32Proxy.Deserialize(inBytes);
            }
        }

        public int BanCmid(string authToken, int cmid)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);
                Int32Proxy.Serialize(bytes, cmid);

                var data = Channel.BanCmid(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return Int32Proxy.Deserialize(inBytes);
            }
        }

        public int BanHwd(string authToken, string hwd)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);
                StringProxy.Serialize(bytes, hwd);

                var data = Channel.BanCmid(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return Int32Proxy.Deserialize(inBytes);
            }
        }

        public int BanIp(string authToken, string ip)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, authToken);
                StringProxy.Serialize(bytes, ip);

                var data = Channel.BanIp(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return Int32Proxy.Deserialize(inBytes);
            }
        }
    }
}
