using System;
using UberStrok.Realtime.Server.Game.Commands;
using UberStrok.Realtime.Server.Game.Events;

namespace UberStrok.Realtime.Server.Game
{
    public class WaitingForPlayersGameState : GameWorldState
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

        public void OnEvent(PlayerLeftEvent @event)
        {

        }

        public override void OnUpdate()
        {
            /* Space */
        }
    }
}
