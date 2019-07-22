using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class EndRoomState : RoomState
    {
        private readonly Countdown _restartCountdown;

        public EndRoomState(GameRoom room) 
            : base(room)
        {
            _restartCountdown = new Countdown(Room.Loop, 5, 0);
            _restartCountdown.Completed += OnRestartCountdownCompleted;
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;

            _restartCountdown.Restart();

            foreach (var otherActor in Room.Actors)
            {
                /* 
                 * This sets the client state to `after round` state which 
                 * clears the GUI.
                 */
                otherActor.Peer.Events.Game.SendTeamWins(Room.Winner);
                otherActor.State.Set(ActorState.Id.End);
            }
        }

        public override void OnTick()
        {
            _restartCountdown.Tick();
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            e.Player.State.Set(ActorState.Id.End);
        }

        private void OnRestartCountdownCompleted()
        {
            /* Reset room state. */
            Room.Reset();
            Room.RoundNumber++;
        }
    }
}
