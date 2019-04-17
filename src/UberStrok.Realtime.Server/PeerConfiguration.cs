namespace UberStrok.Realtime.Server
{
    public class PeerConfiguration
    {
        public int HeartbeatInterval { get; set; }
        public int HeartbeatTimeout { get; set; }

        public byte[] CompositeBytes { get; set; }
        public byte[] JunkBytes { get; set; }
    }
}
