using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGameRoomOperationHandler : BaseOperationHandler<GamePeer>
    {
        protected abstract void OnJump(GamePeer peer, Vector3 position);
        protected abstract void OnUpdatePositionAndRotation(GamePeer peer, Vector3 position, Vector3 velocity, byte horizontalRotation, byte verticalRotation, byte moveState);
        protected abstract void OnChatMessage(GamePeer peer, string message, ChatContext context);
        protected abstract void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes);
        protected abstract void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations);
        protected abstract void OnJoinTeam(GamePeer peer, TeamID team);

        public override void OnOperationRequest(GamePeer peer, byte opCode, MemoryStream bytes)
        {
            var operation = (IGameRoomOperationsType)opCode;
            switch (operation)
            {
                case IGameRoomOperationsType.Jump:
                    Jump(peer, bytes);
                    break;

                case IGameRoomOperationsType.UpdatePositionAndRotation:
                    UpdatePositionAndRotation(peer, bytes);
                    break;

                case IGameRoomOperationsType.ChatMessage:
                    ChatMessage(peer, bytes);
                    break;

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

        private void Jump(GamePeer peer, MemoryStream bytes)
        {
            var position = Vector3Proxy.Deserialize(bytes);

            OnJump(peer, position);
        }

        private void UpdatePositionAndRotation(GamePeer peer, MemoryStream bytes)
        {
            var position = ShortVector3Proxy.Deserialize(bytes);
            var velocity = ShortVector3Proxy.Deserialize(bytes);
            var horizontalRotation = ByteProxy.Deserialize(bytes);
            var verticalRotation = ByteProxy.Deserialize(bytes);
            var moveState = ByteProxy.Deserialize(bytes);

            OnUpdatePositionAndRotation(peer, position, velocity, horizontalRotation, verticalRotation, moveState);
        }

        private void ChatMessage(GamePeer peer, MemoryStream bytes)
        {
            var message = StringProxy.Deserialize(bytes);
            var context = ByteProxy.Deserialize(bytes);

            OnChatMessage(peer, message, (ChatContext)context);
        }

        private void PowerUpRespawnTimes(GamePeer peer, MemoryStream bytes)
        {
            var respawnTimes = ListProxy<ushort>.Deserialize(bytes, UInt16Proxy.Deserialize);

            OnPowerUpRespawnTimes(peer, respawnTimes);
        }

        private void SpawnPositions(GamePeer peer, MemoryStream bytes)
        {
            var team = EnumProxy<TeamID>.Deserialize(bytes);
            var positions = ListProxy<Vector3>.Deserialize(bytes, Vector3Proxy.Deserialize);
            var rotations = ListProxy<byte>.Deserialize(bytes, ByteProxy.Deserialize);

            OnSpawnPositions(peer, team, positions, rotations);
        }

        private void JoinGame(GamePeer peer, MemoryStream bytes)
        {
            var team = EnumProxy<TeamID>.Deserialize(bytes);

            OnJoinTeam(peer, team);
        }
    }
}
