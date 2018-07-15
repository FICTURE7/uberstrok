using UberStrok.Realtime.Server.Game.Commands;

namespace UberStrok.Realtime.Server.Game
{
    public class WaitingForPlayersGameState : GameState
    {
        public WaitingForPlayersGameState()
        {
            /* 
                Register commands which are 
                allowed only in this state.
             */
            Filter.Add<JoinGameCommand>();
            Filter.Enable = true;
        }

        public void OnEvent(PlayerJoinedEvent @event)
        {
            
        }

        public override void OnEnter()
        {
            /* Space */
        }

        public override void OnExit()
        {
            /* Space */
        }

        public override void OnResume()
        {
            /* Space */
        }

        public override void OnUpdate()
        {
            /* Space */
        }
    }
}
