using log4net;
using Photon.SocketServer;
using System.Collections.Generic;
using UberStrok.Core;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeer : BasePeer
    {
        private readonly static ILog Log = LogManager.GetLogger(nameof(GamePeer));

        private readonly GamePeerEvents _events;
        private readonly StateMachine<PeerState.Id> _state;

        public GamePeer(InitRequest request)
            : base(GameApplication.Instance.Configuration.CompositeHashBytes, GameApplication.Instance.Configuration.JunkHashBytes, request)
        {
            KnownActors = new HashSet<int>();
            _events = new GamePeerEvents(this);

            _state = new StateMachine<PeerState.Id>();
            _state.Register(PeerState.Id.None, null);
            _state.Register(PeerState.Id.Overview, new OverviewPeerState(this));
            _state.Register(PeerState.Id.WaitingForPlayers, new WaitingForPlayersPeerState(this));
            _state.Register(PeerState.Id.Countdown, new CountdownPeerState(this));
            _state.Register(PeerState.Id.Playing, new PlayingPeerState(this));
            _state.Register(PeerState.Id.Killed, new KilledPeerState(this));

            AddOperationHandler(new GamePeerOperationHandler());
        }

        /* 
         * For when the peer changes its loadout and the game server is waiting 
         * for the web services to serve back.
         */
        public bool WaitingForLoadout { get; set; }

        public HashSet<int> KnownActors { get; set; }

        public ushort Ping { get; set; }
        public GameActor Actor { get; set; }

        public BaseGameRoom Room { get; set; }
        public LoadoutView Loadout { get; set; }
        public UberstrikeUserView Member { get; set; }

        public GamePeerEvents Events => _events;
        public StateMachine<PeerState.Id> State => _state;

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
            Member = userView;
        }
    }
}
