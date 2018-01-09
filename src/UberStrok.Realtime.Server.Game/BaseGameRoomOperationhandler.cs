using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGameRoomOperationHandler : BaseOperationHandler<GamePeer>
    {
        protected abstract void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes);
        protected abstract void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations);
        protected abstract void OnJoinTeam(GamePeer peer, TeamID team);

        public override void OnOperationRequest(GamePeer peer, byte opCode, MemoryStream bytes)
        {
            var operation = (IGameRoomOperationsType)opCode;
            switch(operation)
            {
                case IGameRoomOperationsType.PowerUpRespawnTimes:
                    PowerUpRespawnTimes(peer, bytes);
                    break;

                case IGameRoomOperationsType.SpawnPositions:
                    SpawnPositions(peer, bytes);
                    break;

                case IGameRoomOperationsType.JoinGame:
                    JoinGame(peer, bytes);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private void PowerUpRespawnTimes(GamePeer peer, MemoryStream bytes)
        {
            var respawnTimes = ListProxy<ushort>.Deserialize(bytes, UInt16Proxy.Deserialize);

            OnPowerUpRespawnTimes(peer, respawnTimes);
        }

        private void SpawnPositions(GamePeer peer,MemoryStream bytes)
        {
            var team = EnumProxy<TeamID>.Deserialize(bytes);
            var positions = ListProxy<Vector3>.Deserialize(bytes, Vector3Proxy.Deserialize);
            var rotations = ListProxy<byte>.Deserialize(bytes, ByteProxy.Deserialize);

            OnSpawnPositions(peer, team, positions, rotations);
        }

        private void JoinGame(GamePeer peer,MemoryStream bytes)
        {
            var team = EnumProxy<TeamID>.Deserialize(bytes);

            OnJoinTeam(peer, team);
        }
    }
}
