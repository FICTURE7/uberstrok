using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomEvents : BaseEventSender
    {
        public GameRoomEvents(GamePeer peer) : base(peer)
        {
            _peer = peer;
        }

        private GamePeer _peer;

        public void SendPlayerJoinGame(GameActorInfoView actor, PlayerMovement movement)
        {
            using (var bytes = new MemoryStream())
            {
                GameActorInfoViewProxy.Serialize(bytes, actor);
                PlayerMovementProxy.Serialize(bytes, movement);

                SendEvent((byte)IGameRoomEventsType.PlayerJoinedGame, bytes);
            }
        }
    }
}
