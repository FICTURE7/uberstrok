using System;

namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomOperationHandler : BaseGameRoomOperationHandler
    {
        public GameRoomOperationHandler(GamePeer peer) : base(peer)
        {
        }

        public override int Id => 0;
    }
}
