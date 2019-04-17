using Photon.SocketServer;
using System.Collections.Generic;
using UberStrok.Core;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeer : Peer
    {
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

        public GamePeerEvents Events { get; }
        public StateMachine<PeerState.Id> State { get; }

        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
            KnownActors = new HashSet<int>();
            Events = new GamePeerEvents(this);

            State = new StateMachine<PeerState.Id>();
            State.Register(PeerState.Id.None, null);
            State.Register(PeerState.Id.Overview, new OverviewPeerState(this));
            State.Register(PeerState.Id.WaitingForPlayers, new WaitingForPlayersPeerState(this));
            State.Register(PeerState.Id.Countdown, new CountdownPeerState(this));
            State.Register(PeerState.Id.Playing, new PlayingPeerState(this));
            State.Register(PeerState.Id.Killed, new KilledPeerState(this));

            Handlers.Add(new GamePeerOperationHandler());
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

        public void UpdateLoadout()
        {
            WaitingForLoadout = true;

            /* Retrieve loadout from web services. */
            var loadout = GetLoadout();
            var weapons = new List<int>
            {
                loadout.MeleeWeapon,
                loadout.Weapon1,
                loadout.Weapon2,
                loadout.Weapon3
            };
            var gear = new List<int>
            {
                loadout.Webbing,
                loadout.Head,
                loadout.Face,
                loadout.Gloves,
                loadout.UpperBody,
                loadout.LowerBody,
                loadout.Boots
            };
            var quickItems = new List<int>
            {
                loadout.QuickItem1,
                loadout.QuickItem2,
                loadout.QuickItem3
            };

            Actor.Info.Weapons = weapons;
            Actor.Info.Gear = gear;
            Actor.Info.QuickItems = quickItems;
            Loadout = loadout;

            WaitingForLoadout = false;
        }

        protected override void OnAuthenticate(UberstrikeUserView userView)
        {
            Member = userView;
            Loadout = GetLoadout();
        }
    }
}
