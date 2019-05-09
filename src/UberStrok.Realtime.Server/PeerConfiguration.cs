using System;
using System.Collections.Generic;

namespace UberStrok.Realtime.Server
{
    public class PeerConfiguration
    {
        public PeerConfiguration(string webServices, string webServicesAuth, int heartbeatInterval, int heartbeatTimeout,
                        IReadOnlyList<byte[]> compositeHashes, IReadOnlyList<byte[]> junkHashes)
        {
            WebServicesAuth = webServicesAuth;
            WebServices = webServices ?? throw new ArgumentNullException(nameof(webServices));
            CompositeHashes = compositeHashes ?? throw new ArgumentNullException(nameof(compositeHashes));
            JunkHashes = junkHashes ?? throw new ArgumentNullException(nameof(junkHashes));

            if (heartbeatInterval <= 0)
                throw new ArgumentOutOfRangeException(nameof(heartbeatInterval));
            if (heartbeatTimeout <= 0)
                throw new ArgumentOutOfRangeException(nameof(heartbeatTimeout));

            HeartbeatInterval = heartbeatInterval;
            HeartbeatTimeout = heartbeatTimeout;
        }

        public string WebServices { get; }
        public string WebServicesAuth { get; }
        public int HeartbeatInterval { get; }
        public int HeartbeatTimeout { get; }
        public IReadOnlyList<byte[]> CompositeHashes { get; }
        public IReadOnlyList<byte[]> JunkHashes { get; }
    }
}
