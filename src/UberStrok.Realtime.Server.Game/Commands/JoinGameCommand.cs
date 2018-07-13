using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game.Commands
{
    public class JoinGameCommand : Command
    {
        /* Team which the player joined. */
        public TeamID Team { get; set; }

        protected override void OnExecute(UberStrok.Game game, GameObject gameObject)
        {
            /* 
                Add a game object which represents 
                the player to the game instance.
             */
            var playerObject = new GameObject(game);
            playerObject.AddComponent<Player>();
        }
    }
}
