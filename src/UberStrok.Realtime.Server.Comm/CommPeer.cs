using Photon.SocketServer;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeer : Peer
    {
        public LobbyRoom Room { get; set; }
        public CommActor Actor { get; set; }
        public CommPeerEvents Events { get; }

        public CommPeer(InitRequest request) : base(request)
        {
            Events = new CommPeerEvents(this);
            Handlers.Add(new CommPeerOperationHandler());
        }

        public override void SendHeartbeat(string hash)
        {
            base.SendHeartbeat(hash);
            Events.SendHeartbeatChallenge(hash);
        }

        public override void SendError(string message = "An error occured that forced UberStrike to halt.")
        {
            base.SendError(message);
            Events.SendDisconnectAndDisablePhoton(message);
        }

        protected override void OnAuthenticate(UberstrikeUserView userView)
        {
            var actorView = new CommActorInfoView
            {
                AccessLevel = userView.CmuneMemberView.PublicProfile.AccessLevel,
                Channel = ChannelType.Steam,
                Cmid = userView.CmuneMemberView.PublicProfile.Cmid,
                PlayerName = userView.CmuneMemberView.PublicProfile.Name,
            };

            Actor = new CommActor(this, actorView);
        }
    }
}
