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

        public void SendMatchStart(int roundNumber, int endTime)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, roundNumber);
                Int32Proxy.Serialize(bytes, endTime);

                SendEvent((byte)IGameRoomEventsType.MatchStart, bytes);
            }
        }

        public void SendMatchStartCountdown(int countdown)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, countdown);

                SendEvent((byte)IGameRoomEventsType.MatchStartCountdown, bytes);
            }
        }

        public void SendPlayerRespawned(int cmid, Vector3 position, byte rotation)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                Vector3Proxy.Serialize(bytes, position);
                ByteProxy.Serialize(bytes, rotation);

                SendEvent((byte)IGameRoomEventsType.PlayerRespawned, bytes);
            }
        }
    }
}
