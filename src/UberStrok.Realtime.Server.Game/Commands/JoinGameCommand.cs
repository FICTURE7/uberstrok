using UberStrok.Core.Common;
using UberStrok.Realtime.Server.Game.Events;

namespace UberStrok.Realtime.Server.Game.Commands
{
    public class JoinGameCommand : Command
    {
        /* Team which the player joined. */
        public TeamID Team { get; set; }

        protected override void OnExecute()
        {
            /* 
                Create a game object which represents 
                the player to the game instance.
             */
            var playerObject = Game.Objects.Create();

            /* Add the components. */
            playerObject.AddComponent<Transform>();
            playerObject.AddComponent<Player>();
            playerObject.AddComponent<PlayerConnection>();

            /* Fire the event, so the current game state can handle it. */
            Game.OnEvent(new PlayerJoinedEvent());
        }
    }
}
