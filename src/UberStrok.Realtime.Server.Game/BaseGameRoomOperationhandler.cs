using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGameRoomOperationHandler : BaseOperationHandler<GamePeer>
    {
        public BaseGameRoomOperationHandler(GamePeer peer) : base(peer)
        {
            // Space
        }

        protected abstract void OnPowerUpRespawnTimes(List<ushort> respawnTimes);
        protected abstract void OnSpawnPositions(TeamID team, List<Vector3> positions, List<byte> rotations);
        protected abstract void OnJoinTeam(TeamID team);

        public override void OnOperationRequest(byte opCode, MemoryStream bytes)
        {
            var operation = (IGameRoomOperationsType)opCode;
            switch(operation)
            {
                case IGameRoomOperationsType.PowerUpRespawnTimes:
                    PowerUpRespawnTimes(bytes);
                    break;

                case IGameRoomOperationsType.SpawnPositions:
                    SpawnPositions(bytes);
                    break;

                case IGameRoomOperationsType.JoinGame:
                    JoinGame(bytes);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void PowerUpRespawnTimes(MemoryStream bytes)
        {
            var respawnTimes = ListProxy<ushort>.Deserialize(bytes, UInt16Proxy.Deserialize);

            OnPowerUpRespawnTimes(respawnTimes);
        }

        private void SpawnPositions(MemoryStream bytes)
        {
            var team = EnumProxy<TeamID>.Deserialize(bytes);
            var positions = ListProxy<Vector3>.Deserialize(bytes, Vector3Proxy.Deserialize);
            var rotations = ListProxy<byte>.Deserialize(bytes, ByteProxy.Deserialize);

            OnSpawnPositions(team, positions, rotations);
        }

        private void JoinGame(MemoryStream bytes)
        {
            var team = EnumProxy<TeamID>.Deserialize(bytes);

            OnJoinTeam(team);
        }
    }
}
