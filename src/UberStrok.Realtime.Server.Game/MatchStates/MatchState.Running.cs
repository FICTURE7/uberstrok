using System;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class RunningMatchState : MatchState
    {
        public RunningMatchState(GameRoom room) 
            : base(room)
        {
            /* Space */
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;
            Room.PlayerLeft += OnPlayerLeft;

            Room.StartTime = Environment.TickCount;
            Room.EndTime = Environment.TickCount + Room.View.TimeLimit * 1000;

            foreach (var player in Room.Players)
                player.State.Set(ActorState.Id.Playing);
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
            Room.PlayerLeft -= OnPlayerLeft;
        }

        public override void OnTick()
        {
            /* Check if match has ended. */
            if (Environment.TickCount >= Room.EndTime)
                Room.State.Set(Id.End);
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            /* Sync the power ups. */
            e.Player.Peer.Events.Game.SendSetPowerUpState(Room.PowerUps.Respawning);
            e.Player.State.Set(ActorState.Id.Playing);

            Room.Spawn(e.Player, out _);
        }

        private void OnPlayerLeft(object sender, PlayerLeftEventArgs e)
        {
            if (Room.Players.Count <= 1)
                Room.State.Set(Id.End);
        }
    }
}
