using System;
using System.IO;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGameRoomOperationHandler : BaseOperationHandler<GamePeer>
    {
        public BaseGameRoomOperationHandler(GamePeer peer) : base(peer)
        {
            // Space
        }

        public override void OnOperationRequest(byte opCode, MemoryStream bytes)
        {
            throw new NotImplementedException();
        }
    }
}
