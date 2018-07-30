using UberStrok.Realtime.Server.Game.Events;

namespace UberStrok.Realtime.Server.Game
{
    public class GameLobbyRoom : Room
    {
        public GameLobbyRoom() : base(0)
        {
            /* Space */
        }

        public void OnEvent(PeerJoinedEvent @event)
        {
            @event.Actor.State = GameActorState.Connected;
        }
    }
}
