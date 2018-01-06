using log4net;
using PhotonHostRuntimeInterfaces;

namespace UberStrok.Realtime.Server.Comm
{
    public class LobbyRoomOperationHandler : BaseLobbyRoomOperationHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommPeerOperationHandler).Name);

        public LobbyRoomOperationHandler(CommPeer peer) : base(peer)
        {
            // Space
        }

        public override void OnChatMessageToAll(string message)
        {
            LobbyManager.Instance.ChatToAll(Peer.Actor, message);
        }

        public override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            Log.Info($"{Peer.Actor.Cmid} Disconnected {reasonCode} -> {reasonDetail}");

            // Remove the peer from the lobby list & update all peer's CommActor list.
            LobbyManager.Instance.Remove(Peer.Actor.Cmid);
            LobbyManager.Instance.UpdateList();
        }
    }
}
