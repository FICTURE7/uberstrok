namespace UberStrok.Realtime.Server.Game
{
    public sealed class CountdownActorState : ActorState
    {
        public CountdownActorState(GameActor actor) 
            : base(actor)
        {
            /* Space */
        }

        public override void OnEnter()
        {
            /* Reset the player statistics when the countdown starts. */
            Actor.Statistics.Reset(hard: true);
            Actor.Info.Health = 100;
            Actor.Info.ArmorPoints = Actor.Info.ArmorPointCapacity;
            Actor.Info.Kills = 0;
            Actor.Info.Deaths = 0;

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
