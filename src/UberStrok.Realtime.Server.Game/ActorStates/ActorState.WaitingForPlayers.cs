namespace UberStrok.Realtime.Server.Game
{
    public sealed class WaitingForPlayersActorState : ActorState
    {
        public WaitingForPlayersActorState(GameActor actor) 
            : base(actor)
        {
            /* Space */
        }

        public override void OnEnter()
        {
            Peer.Events.Game.SendWaitingForPlayer();
        }
    }
}
