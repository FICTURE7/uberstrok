using log4net;

namespace UbzStuff.Realtime.Server.Comm
{
    public class CommPeerOperationHandler : BaseCommPeerOperationHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommPeerOperationHandler));

        public CommPeerOperationHandler(CommPeer peer) : base(peer)
        {
            // Space
        }

        public override int Id => 1;

        public override void OnAuthenticationRequest(string authToken, string magicHash)
        {
            //TODO: Use AuthToken to retrieve data from the web services.
            Log.Info($"Received AuthenticationRequest! {authToken}:{magicHash}");

            Peer.Events.SendLobbyEntered();
            // Send all actors in the lobby to the player.
            Peer.Lobby.Events.SendFullPlayerListUpdate(LobbyRoomManager.Instance.Actors);
        }

        public override void OnSendHeartbeatResponse(string authToken, string responseHash)
        {
            // Space
        }
    }
}
