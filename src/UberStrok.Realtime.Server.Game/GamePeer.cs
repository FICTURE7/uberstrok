using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class GamePeer : Peer
    {
        public GameActor Actor { get; set; }
        public GamePeerEvents Events { get; }

        private LoadoutView LoadoutView { get; set; }

        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
            Events = new GamePeerEvents(this);
            Handlers.Add(GamePeerOperationHandler.Instance);

            GameApplication.Instance.Lobby.Join(this);
        }

        public override void SendHeartbeat(string hash)
        {
            Events.SendHeartbeatChallenge(hash);
        }

        public override void SendError(string message = "An error occured that forced UberStrike to halt.")
        {
            base.SendError(message);
            Events.SendDisconnectAndDisablePhoton(message);
        }

        public LoadoutView GetLoadout(bool retrieve)
        {
            if (retrieve || LoadoutView == null)
            {
                /* Retrieve loadout data from the web server. */
                Log.Debug($"Retrieving Loadout from {Configuration.WebServices}");
                LoadoutView = new UserWebServiceClient(Configuration.WebServices).GetLoadoutServer(Configuration.WebServicesAuth, AuthToken);
            }

            return LoadoutView;
        }

        protected override void OnAuthenticate(UberstrikeUserView userView)
        {
            GetLoadout(retrieve: true);
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            base.OnDisconnect(reasonCode, reasonDetail);

            GameApplication.Instance.Lobby.Leave(this);
        }
    }
}
