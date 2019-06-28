using log4net;
using System;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameActor
    {
        protected ILog Log { get; }

        public int Cmid => Info.Cmid;

        public int PlayerId
        {
            get => Info.PlayerId;
            set
            {
                Info.PlayerId = (byte)value;
                Movement.PlayerId = (byte)value;
            }
        }

        public string PlayerName
        {
            get => Info.PlayerName;
            set => Info.PlayerName = value;
        }

        public DateTime DateJoined { get; set; }

        public StatisticsManager Statistics { get; private set; }

        public DamageEventView Damages { get; }
        public PlayerMovement Movement { get; }
        public GameActorInfo Info { get; }

        public ProjectileManager Projectiles { get; }
        public PingManager Ping { get; }

        public TrustManager Trust { get; }
        public LoadoutManager Loadout { get; }

        public GamePeer Peer { get; }
        public GameRoom Room { get; }
        public StateMachine<ActorState.Id> State { get; }
        
        public GameActor(GamePeer peer, GameRoom room)
        {
            Peer = peer ?? throw new ArgumentNullException(nameof(peer));
            Room = room ?? throw new ArgumentNullException(nameof(peer));

            Log = LogManager.GetLogger(nameof(GameActor));

            Info = new GameActorInfo();
            Movement = new PlayerMovement();
            Damages = new DamageEventView();

            Ping = new PingManager();
            Projectiles = new ProjectileManager();

            Trust = new TrustManager();
            Loadout = new LoadoutManager();

            State = new StateMachine<ActorState.Id>();
            State.Register(ActorState.Id.None, null);
            State.Register(ActorState.Id.Overview, new OverviewActorState(this));
            State.Register(ActorState.Id.WaitingForPlayers, new WaitingForPlayersActorState(this));
            State.Register(ActorState.Id.Countdown, new CountdownActorState(this));
            State.Register(ActorState.Id.Playing, new PlayingActorState(this));
            State.Register(ActorState.Id.Killed, new KilledActorState(this));
            State.Register(ActorState.Id.End, new EndActorState(this));

            Reset();
        }

        public void Tick()
        {
            Peer.Tick();
            State.Tick();
            Trust.Tick();
        }

        public void Reset()
        {
            var userView = Peer.GetUser(retrieve: false);
            var loadoutView = Peer.GetLoadout(retrieve: false);
            var profileView = userView.CmuneMemberView.PublicProfile;
            var statsView = userView.UberstrikeMemberView.PlayerStatisticsView;

            Loadout.Update(Room.Shop, loadoutView);

            Info.PlayerName = profileView.Name;
            Info.PlayerState = PlayerStates.None;
            Info.TeamID = TeamID.NONE;
            Info.Channel = ChannelType.Steam;
            Info.Level = statsView.Level == 0 ? 1 : statsView.Level;
            Info.Cmid = profileView.Cmid;
            Info.ClanTag = profileView.GroupTag;
            Info.AccessLevel = profileView.AccessLevel;

            Info.Gear = Loadout.Gear.GetAsList();
            Info.Weapons = Loadout.Weapons.GetAsList();
            Info.QuickItems = Loadout.QuickItems.GetAsList();

            Info.Health = 100;
            Info.ArmorPointCapacity = Loadout.Gear.GetArmorCapacity();
            Info.ArmorPoints = Info.ArmorPointCapacity;

            Info.Kills = 0;
            Info.Deaths = 0;

            /* Ignore these changes. */
            Info.GetViewDelta().Reset();

            State.Reset();
            Loadout.Reset();

            Statistics = new StatisticsManager();

            Log.Info($"{GetDebug()} rested with armor capacity {Info.ArmorPointCapacity}.");
        }

        public string GetDebug()
        {
            return $"(actor <{Cmid}> \"{PlayerName}\":{PlayerId} state {State.Current})";
        }
    }
}
