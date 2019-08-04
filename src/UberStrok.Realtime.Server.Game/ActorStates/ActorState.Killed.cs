using UberStrok.Core;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class KilledActorState : ActorState
    {
        private readonly Countdown _respawnCountdown;
        private readonly Countdown _disconnectCountdown;

        public KilledActorState(GameActor actor) 
            : base(actor)
        {
            _respawnCountdown = new Countdown(Room.Loop, 5, 0);
            _respawnCountdown.Counted += OnRespawnCountdownCounted;
            _respawnCountdown.Completed += OnRespawnCountdownCompleted;

            _disconnectCountdown = new Countdown(Room.Loop, 60 * 3, 0);
            _disconnectCountdown.Counted += OnDisconnectCountdownCounted;
            _disconnectCountdown.Completed += OnDisconnectCountdownCompleted;
        }

        public override void OnEnter()
        {
            Actor.Info.Health = 0;
            Actor.Info.ArmorPoints = 0;
            Actor.Info.PlayerState |= PlayerStates.Dead;

            /* Reset current statistics view. */
            Actor.Statistics.Reset(hard: false);

            _respawnCountdown.Restart();
            _disconnectCountdown.Restart();
        }

        public override void OnTick()
        {
            _respawnCountdown.Tick();
            _disconnectCountdown.Tick();
        }

        public override void OnExit()
        {
            Actor.Info.Health = 100;
            Actor.Info.ArmorPoints = Actor.Info.ArmorPointCapacity;
            Actor.Info.PlayerState &= ~PlayerStates.Dead;
        }

        private void OnRespawnCountdownCounted(int count)
        {
            Peer.Events.Game.SendPlayerRespawnCountdown(count);
        }

        private void OnRespawnCountdownCompleted()
        {
            /* 
             * Start disconnect countdown after the respawn countdown is done
             * and is NOT in `waiting for players` match state.
             */
            if (Room.State.Current != RoomState.Id.WaitingForPlayers)
                _disconnectCountdown.Restart();
        }

        private void OnDisconnectCountdownCounted(int count)
        {
            /* Start sending count after 30th countdown. */
            if (count <= 30)
                Peer.Events.Game.SendDisconnectCountdown(count);
        }

        private void OnDisconnectCountdownCompleted()
        {
            Peer.Disconnect();
        }
    }
}
