using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class PhotonServerLoadView
    {
        public int PeersConnected { get; set; }
        public int PlayersConnected { get; set; }
        public int RoomsCreated { get; set; }
        public float MaxPlayerCount { get; set; }
        public int Latency { get; set; }
        public DateTime TimeStamp { get; set; }
        public Status State { get; set; }

        public enum Status
        {
            None,
            Alive,
            NotReachable
        }
    }
}
