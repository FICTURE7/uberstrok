using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Server.Game.Core
{
    public abstract class GameRoomOperationHandler : BaseOperationHandler<GamePeer>
    {
        protected abstract void OnPowerUpPicked(GamePeer peer, int pickupId, byte type, byte value);
        protected abstract void OnRemoveProjectile(GamePeer peer, int projectileId, bool explode);
        protected abstract void OnEmitProjectile(GamePeer peer, Vector3 origin, Vector3 direction, byte slot, int projectileId, bool explode);
        protected abstract void OnRespawnRequest(GamePeer peer);
        protected abstract void OnExplosionDamage(GamePeer peer, int target, byte slot, byte distance, Vector3 force);
        protected abstract void OnDirectHitDamage(GamePeer peer, int target, byte bodyPart, byte bullets);
        protected abstract void OnDirectDamage(GamePeer peer, ushort damage);
        protected abstract void OnSwitchWeapon(GamePeer peer, byte slot);
        protected abstract void OnSingleBulletFire(GamePeer peer);
        protected abstract void OnIsPaused(GamePeer peer, bool on);
        protected abstract void OnIsInSniperMode(GamePeer peer, bool on);
        protected abstract void OnIsFiring(GamePeer peer, bool on);
        protected abstract void OnJump(GamePeer peer, Vector3 position);
        protected abstract void OnUpdatePositionAndRotation(GamePeer peer, Vector3 position, Vector3 velocity, byte horizontalRotation, byte verticalRotation, byte moveState);
        protected abstract void OnChatMessage(GamePeer peer, string message, ChatContext context);
        protected abstract void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes);
        protected abstract void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations);
        protected abstract void OnJoinGame(GamePeer peer, TeamID team);

        public override void OnOperationRequest(GamePeer peer, byte opCode, MemoryStream bytes)
        {
            var operation = (IGameRoomOperationsType)opCode;
            switch (operation)
            {
                case IGameRoomOperationsType.PowerUpPicked:
                    PowerUpPicked(peer, bytes);
                    break;

                case IGameRoomOperationsType.SingleBulletFire:
                    SingleBulletFire(peer, bytes);
                    break;

                case IGameRoomOperationsType.RemoveProjectile:
                    RemoveProjectile(peer, bytes);
                    break;

                case IGameRoomOperationsType.EmitProjectile:
                    EmitProjectile(peer, bytes);
                    break;

                case IGameRoomOperationsType.RespawnRequest:
                    RespawnRequest(peer, bytes);
                    break;

                case IGameRoomOperationsType.ExplosionDamage:
                    ExplosionDamage(peer, bytes);
                    break;

                case IGameRoomOperationsType.DirectHitDamage:
                    DirectHitDamage(peer, bytes);
                    break;

                case IGameRoomOperationsType.DirectDamage:
                    DirectDamage(peer, bytes);
                    break;

                case IGameRoomOperationsType.SwitchWeapon:
                    SwitchWeapon(peer, bytes);
                    break;

                case IGameRoomOperationsType.IsPaused:
                    IsPaused(peer, bytes);
                    break;

                case IGameRoomOperationsType.IsInSniperMode:
                    IsInSniperMode(peer, bytes);
                    break;

                case IGameRoomOperationsType.IsFiring:
                    IsFiring(peer, bytes);
                    break;

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

        private void PowerUpPicked(GamePeer peer, MemoryStream bytes)
        {
            var pickupId = Int32Proxy.Deserialize(bytes);
            var type = ByteProxy.Deserialize(bytes);
            var value = ByteProxy.Deserialize(bytes);

            OnPowerUpPicked(peer, pickupId, type, value);
        }

        private void IsInSniperMode(GamePeer peer, MemoryStream bytes)
        {
            var on = BooleanProxy.Deserialize(bytes);

            OnIsInSniperMode(peer, on);
        }

        private void SingleBulletFire(GamePeer peer, MemoryStream bytes)
        {
            OnSingleBulletFire(peer);
        }

        private void RemoveProjectile(GamePeer peer, MemoryStream bytes)
        {
            var projectileId = Int32Proxy.Deserialize(bytes);
            var explode = BooleanProxy.Deserialize(bytes);

            OnRemoveProjectile(peer, projectileId, explode);
        }

        private void EmitProjectile(GamePeer peer, MemoryStream bytes)
        {
            var origin = Vector3Proxy.Deserialize(bytes);
            var direction = Vector3Proxy.Deserialize(bytes);
            var slot = ByteProxy.Deserialize(bytes);
            var projectileId = Int32Proxy.Deserialize(bytes);
            var explode = BooleanProxy.Deserialize(bytes);

            OnEmitProjectile(peer, origin, direction, slot, projectileId, explode);
        }

        private void RespawnRequest(GamePeer peer, MemoryStream bytes)
        {
            OnRespawnRequest(peer);
        }

        private void ExplosionDamage(GamePeer peer, MemoryStream bytes)
        {
            var target = Int32Proxy.Deserialize(bytes);
            var slot = ByteProxy.Deserialize(bytes);
            var distance = ByteProxy.Deserialize(bytes);
            var force = Vector3Proxy.Deserialize(bytes);

            OnExplosionDamage(peer, target, slot, distance, force);
        }

        private void DirectHitDamage(GamePeer peer, MemoryStream bytes)
        {
            var target = Int32Proxy.Deserialize(bytes);
            var bodyPart = ByteProxy.Deserialize(bytes);
            var bullets = ByteProxy.Deserialize(bytes);

            OnDirectHitDamage(peer, target, bodyPart, bullets);
        }

        private void DirectDamage(GamePeer peer, MemoryStream bytes)
        {
            var damage = UInt16Proxy.Deserialize(bytes);

            OnDirectDamage(peer, damage);
        }

        private void SwitchWeapon(GamePeer peer, MemoryStream bytes)
        {
            var slot = ByteProxy.Deserialize(bytes);

            OnSwitchWeapon(peer, slot);
        }

        private void IsPaused(GamePeer peer, MemoryStream bytes)
        {
            var on = BooleanProxy.Deserialize(bytes);

            OnIsPaused(peer, on);
        }

        private void IsFiring(GamePeer peer, MemoryStream bytes)
        {
            var on = BooleanProxy.Deserialize(bytes);

            OnIsFiring(peer, on);
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

            OnJoinGame(peer, team);
        }
    }
}
