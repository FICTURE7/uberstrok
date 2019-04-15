using Newtonsoft.Json;
using System.Text;

namespace UberStrok.Realtime.Server.Game
{
    public class GameConfiguration
    {
        public static readonly GameConfiguration Default = new GameConfiguration
        {
            CompositeHash = null,
            JunkHash = null
        };

        private byte[] _compositeHashBytes;
        private byte[] _junkHashBytes;

        [JsonProperty("composite_hash")]
        public string CompositeHash { get; set; }
        [JsonProperty("junk_hash")]
        public string JunkHash { get; set; }

        public byte[] CompositeHashBytes
        {
            get
            {
                if (_compositeHashBytes == null && CompositeHash != null)
                    _compositeHashBytes = Encoding.ASCII.GetBytes(CompositeHash);

                return _compositeHashBytes;
            }
        }

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
