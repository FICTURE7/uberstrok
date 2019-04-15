using log4net;
using System;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeerOperationHandler : BaseCommPeerOperationHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(CommPeerOperationHandler));

        public CommPeerOperationHandler()
        {
            // Space
        }

        public override void OnAuthenticationRequest(CommPeer peer, string authToken, string magicHash)
        {
            if (!peer.Authenticate(authToken, magicHash))
            {
                peer.Events.SendDisconnectAndDisablePhoton();
                return;
            }

            if (CommApplication.Instance.Configuration.JunkHash != null &&
                peer.Actor.AccessLevel != MemberAccessLevel.Admin)
                peer.Challenge();

            /* Make peer join the global lobby room. */
            CommApplication.Instance.Rooms.Global.Join(peer);
        }

        public override void OnSendHeartbeatResponse(CommPeer peer, string authToken, string responseHash)
        {
            try
            {
                Log.Info($"Checking challenge {responseHash}.");
                if (!peer.ChallengeCheck(responseHash))
                    peer.Events.SendDisconnectAndDisablePhoton();
            }
            catch (Exception ex)
            {
                Log.Error("Failed to check challenge.", ex);
                peer.Events.SendDisconnectAndDisablePhoton();
            }
        }
    }
}
