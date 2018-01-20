namespace UberStrok.Realtime.Server.Game
{
    public class WaitingForPlayersMatchState : MatchState
    {
        public WaitingForPlayersMatchState(BaseGameRoom room) : base(room)
        {
            // Space
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
        }

        public override void OnUpdate()
        {
            // Space
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            var player = e.Player;

            /* If we got more than 1 player we start the countdown. */
            if (Room.Players.Count > 1)
                Room.State.Set(Id.Countdown);
            else
            {
                /* Let all peers know that the client has joined. */
                foreach (var otherPeer in Room.Peers)
                {
                    otherPeer.Events.Game.SendPlayerJoinedGame(player.Actor.Info.View, player.Actor.Movement);
                    otherPeer.KnownActors.Add(player.Actor.Cmid);
                }

                /* Set the player in a 'waiting for players' state. */
                player.State.Set(PeerState.Id.WaitingForPlayers);
            }
        }
    }
}
