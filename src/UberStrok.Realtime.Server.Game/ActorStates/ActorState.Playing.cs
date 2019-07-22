using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class PlayingActorState : ActorState
    {
        private readonly FixedTimer _overflowTimer;

        public PlayingActorState(GameActor actor)
            : base(actor)
        {
            _overflowTimer = new FixedTimer(Room.Loop, 1000f);
        }

        public override void OnEnter()
        {
            Actor.TimePlayed = Room.Loop.Time;

            /* 
             * This sets the client's match and player state to `match running`
             * state which is the equivalent of `playing` state.
             *
             * The client does not care about the roundNumber apparently (in
             * TeamDeathMatch atleast).
             */
            Peer.Events.Game.SendMatchStart(Room.RoundNumber, Room.EndTime);
        }

        public override void OnTick()
        {
            int healthCapacity = 100;
            int armorCapacity = Peer.Actor.Info.ArmorPointCapacity;

            _overflowTimer.IsEnabled = Peer.Actor.Info.Health > healthCapacity || Peer.Actor.Info.ArmorPoints > armorCapacity;

            while (_overflowTimer.Tick())
            {
                if (Peer.Actor.Info.Health > healthCapacity)
                    Peer.Actor.Info.Health--;
                if (Peer.Actor.Info.ArmorPoints > armorCapacity)
                    Peer.Actor.Info.ArmorPoints--;
            }
        }

        public override void OnResume()
        {
            _overflowTimer.Reset();
        }
    }
}
