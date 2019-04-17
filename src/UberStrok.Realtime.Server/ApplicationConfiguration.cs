using Newtonsoft.Json;
using System.Text;

namespace UberStrok.Realtime.Server
{
    public class ApplicationConfiguration
    {
        public static readonly ApplicationConfiguration Default = new ApplicationConfiguration
        {
            WebServices = "http://localhost/2.0/",
            HeartbeatTimeout = 5,
            HeartbeatInterval = 5,
            CompositeHash = null,
            JunkHash = null
        };

        private byte[] _compositeHashBytes;
        private byte[] _junkHashBytes;

        [JsonRequired]
        [JsonProperty("webservices")]
        public string WebServices { get; private set; }

        [JsonProperty("heartbeat_timeout")]
        public int HeartbeatTimeout { get; private set; }
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; private set; }

        [JsonProperty("composite_hash")]
        public string CompositeHash { get; private set; }
        [JsonProperty("junk_hash")]
        public string JunkHash { get; private set; }

        [JsonIgnore]
        public byte[] CompositeHashBytes
        {
            get
            {
                if (_compositeHashBytes == null && CompositeHash != null)
                    _compositeHashBytes = Encoding.ASCII.GetBytes(CompositeHash);

                return _compositeHashBytes;
            }
        }

        [JsonIgnore]
        public byte[] JunkHashBytes
        {
            get
            {
                if (_junkHashBytes== null && JunkHash != null)
                    _junkHashBytes = Encoding.ASCII.GetBytes(JunkHash);

                return _junkHashBytes;
            }
        }
    }
}
