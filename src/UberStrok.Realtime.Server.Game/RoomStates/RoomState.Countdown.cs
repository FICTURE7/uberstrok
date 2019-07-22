using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class CountdownRoomState : RoomState
    {
        private readonly Countdown _countdown;

        public CountdownRoomState(GameRoom room) 
            : base(room)
        {
            _countdown = new Countdown(Room.Loop, 5, 0);
            _countdown.Counted += OnCountdownCounted;
            _countdown.Completed += OnCountdownCompleted;
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;

            /* 
             * Prepare all players by placing them in a 'prepare for next round state',
             * and spawning them.
             */
            foreach (var player in Room.Players)
                player.State.Set(ActorState.Id.Countdown);


            /* Reset all power ups in the map. */
            Room.PowerUps.Reset();
            foreach (var actor in Room.Actors)
                actor.Peer.Events.Game.SendResetAllPowerUps();

            _countdown.Restart();
        }

        public override void OnTick()
        {
            _countdown.Tick();
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            e.Player.State.Set(ActorState.Id.Countdown);
        }

        private void OnCountdownCounted(int count)
        {
            foreach (var otherActor in Room.Actors)
                otherActor.Peer.Events.Game.SendMatchStartCountdown(count);
        }

        private void OnCountdownCompleted()
        {
            Room.State.Set(Id.Running);
        }
    }
}
