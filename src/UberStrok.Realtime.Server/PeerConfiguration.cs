namespace UberStrok.Realtime.Server
{
    public class PeerConfiguration
    {
        public string WebServices { get; set; }
        public int HeartbeatInterval { get; set; }
        public int HeartbeatTimeout { get; set; }
        public byte[] CompositeHashBytes { get; set; }
        public byte[] JunkHashBytes { get; set; }
    }
}
