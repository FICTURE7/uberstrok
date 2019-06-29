namespace UberStrok.Realtime.Server.Game
{
    public sealed class CountdownActorState : ActorState
    {
        public CountdownActorState(GameActor actor) : base(actor)
        {
            /* Space */
        }

        public override void OnEnter()
        {
            /* 
             * This sets the client's match and player state to 
             * `prepare for next round` state which is the equivalent of
             * Countdown state.
             */
            Peer.Events.Game.SendPrepareNextRound();

            /* Reset score board. */
            Peer.Events.Game.SendUpdateRoundScore(Room.RoundNumber, default, default);
            Peer.Events.Game.SendKillsRemaining(default, default);

            /* Spawn player in a random spot. */
            Room.Spawn(Actor);
        }
    }
}
