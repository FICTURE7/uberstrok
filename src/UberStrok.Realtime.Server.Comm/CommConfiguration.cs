using Newtonsoft.Json;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommConfiguration
    {
        public static readonly CommConfiguration Default = new CommConfiguration
        {
            CompositeHash = null,
            JunkHash = null
        };

        [JsonProperty("composite_hash")]
        public string CompositeHash { get; set; }
        [JsonProperty("junk_hash")]
        public string JunkHash { get; set; }
    }
}
