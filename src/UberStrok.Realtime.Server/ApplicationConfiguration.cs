using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            _compositeHashes = new List<string>(),
            _junkHashes = new List<string>()
        };

        [JsonRequired]
        [JsonProperty("composite_hash")]
        private List<string> _compositeHashes;
        [JsonRequired]
        [JsonProperty("junk_hash")]
        private List<string> _junkHashes;

        [JsonRequired]
        [JsonProperty("webservices")]
        public string WebServices { get; private set; }

        [JsonProperty("heartbeat_timeout")]
        public int HeartbeatTimeout { get; private set; }
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; private set; }

        [JsonIgnore]
        public List<byte[]> CompositeHashBytes { get; } = new List<byte[]>();
        [JsonIgnore]
        public List<byte[]> JunkHashBytes { get; } = new List<byte[]>();

        public void Check()
        {
            if (HeartbeatInterval < 0 || HeartbeatTimeout < 0)
                throw new FormatException("HeartbeatInterval and HeartbeatTimeout cannot be less than 0.");

            if (HeartbeatInterval == 0)
                HeartbeatInterval = 5;
            if (HeartbeatTimeout == 0)
                HeartbeatTimeout = 5;

            CheckHashes(_compositeHashes, CompositeHashBytes);
            CheckHashes(_junkHashes, JunkHashBytes);
        }

        private void CheckHashes(List<string> hashes, List<byte[]> hashBytes)
        {
            const string VALID_CHARS = "0123456789abcdef";

            foreach (var hash in hashes)
            {
                if (hash.Length != 64)
                    throw new FormatException("Hash must be 64 character long");

                foreach (var c in hash)
                {
                    if (!VALID_CHARS.Contains(c.ToString()))
                        throw new FormatException("Hash contains invalid characters");
                }

                hashBytes.Add(Encoding.ASCII.GetBytes(hash));
            }
        }
    }
}
