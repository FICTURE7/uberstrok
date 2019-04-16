using log4net;
using System;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeerOperationHandler : BaseCommPeerOperationHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(CommPeerOperationHandler));

        public override void OnAuthenticationRequest(CommPeer peer, string authToken, string magicHash)
        {
            if (!peer.Authenticate(authToken, magicHash))
            {
                peer.SendError();
                return;
            }

            CommApplication.Instance.Rooms.Global.Join(peer);
        }

        public override void OnSendHeartbeatResponse(CommPeer peer, string authToken, string responseHash)
        {
            try
            {
                if (!peer.HeartbeatCheck(responseHash))
                    peer.SendError();
            }
            catch (Exception ex)
            {
                Log.Error("Exception while checking heartbeat", ex);
                peer.SendError();
            }
        }
    }
}
