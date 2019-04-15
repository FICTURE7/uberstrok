using log4net;
using Photon.SocketServer;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeer : BasePeer
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(CommPeer));

        public LobbyRoom Room { get; set; }
        public CommActor Actor { get; set; }
        public CommPeerEvents Events { get; }

        public CommPeer(InitRequest request) 
            : base(CommApplication.Instance.Configuration.CompositeHashBytes, CommApplication.Instance.Configuration.JunkHashBytes, request)
        {
            Events = new CommPeerEvents(this);
            AddOperationHandler(new CommPeerOperationHandler());
        }

        public override void DoHeartbeat(string hash)
        {
            base.DoHeartbeat(hash);
            Events.SendHeartbeatChallenge(hash);
        }

        public override void DoError(string message = "An error occured that forced UberStrike to halt.")
        {
            base.DoError(message);
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
