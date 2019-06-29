using System;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class PlayingActorState : ActorState
    {
        private readonly Timer _overflowTimer;

        public PlayingActorState(GameActor actor) : base(actor)
        {
            _overflowTimer = new Timer(Room.Loop, TimeSpan.FromSeconds(1));
            _overflowTimer.Tick += OnOverflowTick;
        }

        public override void OnEnter()
        {
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
            _overflowTimer.Update();
        }

        public override void OnResume()
        {
            _overflowTimer.Reset();
        }

        private void OnOverflowTick()
        {
            int healthCapacity = 100;
            int armorCapacity = Peer.Actor.Info.ArmorPointCapacity;

            if (Peer.Actor.Info.Health > healthCapacity)
                Peer.Actor.Info.Health--;
            if (Peer.Actor.Info.ArmorPoints > armorCapacity)
                Peer.Actor.Info.ArmorPoints--;
        }
    }
}
