using System;

namespace UbzStuff.Realtime.Client
{
    public class LobbyRoomOperations
    {
        public LobbyRoomOperations(BasePeer peer, byte id)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _id = id;
            _peer = peer;
        }

        private byte _id;
        private readonly BasePeer _peer;
    }
}
