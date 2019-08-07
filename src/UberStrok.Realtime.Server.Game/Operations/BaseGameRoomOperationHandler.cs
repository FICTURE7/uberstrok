using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Common;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGameRoomOperationHandler : OperationHandler<GamePeer>
    {
        public sealed override byte Id => 0;

        protected abstract void OnPowerUpPicked(GameActor actor, int pickupId, PickupItemType type, byte value);
        protected abstract void OnRemoveProjectile(GameActor actor, int projectileId, bool explode);
        protected abstract void OnEmitProjectile(GameActor actor, Vector3 origin, Vector3 direction, byte slot, int projectileId, bool explode);
        protected abstract void OnEmitQuickItem(GameActor actor, Vector3 origin, Vector3 direction, int itemId, byte playerNumber, int projectileId);
        protected abstract void OnRespawnRequest(GameActor actor);
        protected abstract void OnExplosionDamage(GameActor actor, int target, byte slot, byte distance, Vector3 force);
        protected abstract void OnDirectHitDamage(GameActor actor, int target, byte bodyPart, byte bullets);
        protected abstract void OnDirectDamage(GameActor actor, ushort damage);
        protected abstract void OnDirectDeath(GameActor actor);
        protected abstract void OnSwitchWeapon(GameActor actor, byte slot);
        protected abstract void OnSingleBulletFire(GameActor actor);
        protected abstract void OnIsPaused(GameActor actor, bool on);
        protected abstract void OnIsInSniperMode(GameActor actor, bool on);
        protected abstract void OnIsFiring(GameActor actor, bool on);
        protected abstract void OnJump(GameActor actor, Vector3 position);
        protected abstract void OnUpdatePositionAndRotation(GameActor actor, Vector3 position, Vector3 velocity, byte horizontalRotation, byte verticalRotation, byte moveState);
        protected abstract void OnChatMessage(GameActor actor, string message, byte context);
        protected abstract void OnPowerUpRespawnTimes(GameActor actor, List<ushort> respawnTimes);
        protected abstract void OnSpawnPositions(GameActor actor, TeamID team, List<Vector3> positions, List<byte> rotations);
        protected abstract void OnJoinTeam(GameActor actor, TeamID team);
        protected abstract void OnOpenDoor(GameActor actor, int doorId);
        protected abstract void OnHitFeedback(GameActor actor, int targetCmid, Vector3 force);

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
                case IGameRoomOperationsType.EmitQuickItem:
                    EmitQuickItem(peer, bytes);
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
                case IGameRoomOperationsType.DirectDeath:
                    DirectDeath(peer, bytes);
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
                case IGameRoomOperationsType.OpenDoor:
                    OpenDoor(peer, bytes);
                    break;
                case IGameRoomOperationsType.HitFeedback:
                    HitFeedback(peer, bytes);
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

            Enqueue(() => OnPowerUpPicked(peer.Actor, pickupId, (PickupItemType)type, value));
        }

        private void IsInSniperMode(GamePeer peer, MemoryStream bytes)
        {
            var on = BooleanProxy.Deserialize(bytes);

            Enqueue(() => OnIsInSniperMode(peer.Actor, on));
        }

        private void SingleBulletFire(GamePeer peer, MemoryStream _)
        {
            Enqueue(() => OnSingleBulletFire(peer.Actor));
        }

        private void RemoveProjectile(GamePeer peer, MemoryStream bytes)
        {
            var projectileId = Int32Proxy.Deserialize(bytes);
            var explode = BooleanProxy.Deserialize(bytes);

            Enqueue(() => OnRemoveProjectile(peer.Actor, projectileId, explode));
        }

        private void EmitProjectile(GamePeer peer, MemoryStream bytes)
        {
            var origin = Vector3Proxy.Deserialize(bytes);
            var direction = Vector3Proxy.Deserialize(bytes);
            var slot = ByteProxy.Deserialize(bytes);
            var projectileId = Int32Proxy.Deserialize(bytes);
            var explode = BooleanProxy.Deserialize(bytes);

            Enqueue(() => OnEmitProjectile(peer.Actor, origin, direction, slot, projectileId, explode));
        }

        private void EmitQuickItem(GamePeer peer, MemoryStream bytes)
        {
            var origin = Vector3Proxy.Deserialize(bytes);
            var direction = Vector3Proxy.Deserialize(bytes);
            var itemId = Int32Proxy.Deserialize(bytes);
            var playerNumber = ByteProxy.Deserialize(bytes);
            var projectileId = Int32Proxy.Deserialize(bytes);

            Enqueue(() => OnEmitQuickItem(peer.Actor, origin, direction, itemId, playerNumber, projectileId));
        }

        private void RespawnRequest(GamePeer peer, MemoryStream _)
        {
            Enqueue(() => OnRespawnRequest(peer.Actor));
        }

        private void ExplosionDamage(GamePeer peer, MemoryStream bytes)
        {
            var target = Int32Proxy.Deserialize(bytes);
            var slot = ByteProxy.Deserialize(bytes);
            var distance = ByteProxy.Deserialize(bytes);
            var force = Vector3Proxy.Deserialize(bytes);

            Enqueue(() => OnExplosionDamage(peer.Actor, target, slot, distance, force));
        }

        private void DirectHitDamage(GamePeer peer, MemoryStream bytes)
        {
            var target = Int32Proxy.Deserialize(bytes);
            var bodyPart = ByteProxy.Deserialize(bytes);
            var bullets = ByteProxy.Deserialize(bytes);

            Enqueue(() => OnDirectHitDamage(peer.Actor, target, bodyPart, bullets));
        }

        private void DirectDamage(GamePeer peer, MemoryStream bytes)
        {
            var damage = UInt16Proxy.Deserialize(bytes);

            Enqueue(() => OnDirectDamage(peer.Actor, damage));
        }

        private void DirectDeath(GamePeer peer, MemoryStream _)
        {
            Enqueue(() => OnDirectDeath(peer.Actor));
        }

        private void SwitchWeapon(GamePeer peer, MemoryStream bytes)
        {
            var slot = ByteProxy.Deserialize(bytes);

            Enqueue(() => OnSwitchWeapon(peer.Actor, slot));
        }

        private void IsPaused(GamePeer peer, MemoryStream bytes)
        {
            var on = BooleanProxy.Deserialize(bytes);

            Enqueue(() => OnIsPaused(peer.Actor, on));
        }

        private void IsFiring(GamePeer peer, MemoryStream bytes)
        {
            var on = BooleanProxy.Deserialize(bytes);

            Enqueue(() => OnIsFiring(peer.Actor, on));
        }

        private void Jump(GamePeer peer, MemoryStream bytes)
        {
            var position = Vector3Proxy.Deserialize(bytes);

            Enqueue(() => OnJump(peer.Actor, position));
        }

        private void UpdatePositionAndRotation(GamePeer peer, MemoryStream bytes)
        {
            var position = ShortVector3Proxy.Deserialize(bytes);
            var velocity = ShortVector3Proxy.Deserialize(bytes);
            var horizontalRotation = ByteProxy.Deserialize(bytes);
            var verticalRotation = ByteProxy.Deserialize(bytes);
            var moveState = ByteProxy.Deserialize(bytes);

            Enqueue(() => OnUpdatePositionAndRotation(peer.Actor, position, velocity, horizontalRotation, verticalRotation, moveState));
        }

        private void ChatMessage(GamePeer peer, MemoryStream bytes)
        {
            var message = StringProxy.Deserialize(bytes);
            var context = ByteProxy.Deserialize(bytes);

            Enqueue(() => OnChatMessage(peer.Actor, message, context));
        }

        private void PowerUpRespawnTimes(GamePeer peer, MemoryStream bytes)
        {
            var respawnTimes = ListProxy<ushort>.Deserialize(bytes, UInt16Proxy.Deserialize);

            Enqueue(() => OnPowerUpRespawnTimes(peer.Actor, respawnTimes));
        }

        private void SpawnPositions(GamePeer peer, MemoryStream bytes)
        {
            var team = EnumProxy<TeamID>.Deserialize(bytes);
            var positions = ListProxy<Vector3>.Deserialize(bytes, Vector3Proxy.Deserialize);
            var rotations = ListProxy<byte>.Deserialize(bytes, ByteProxy.Deserialize);

            Enqueue(() => OnSpawnPositions(peer.Actor, team, positions, rotations));
        }

        private void JoinGame(GamePeer peer, MemoryStream bytes)
        {
            var team = EnumProxy<TeamID>.Deserialize(bytes);

            Enqueue(() => OnJoinTeam(peer.Actor, team));
        }

        private void OpenDoor(GamePeer peer, MemoryStream bytes)
        {
            var doorId = Int32Proxy.Deserialize(bytes);

            Enqueue(() => OnOpenDoor(peer.Actor, doorId));
        }

        private void HitFeedback(GamePeer peer, MemoryStream bytes)
        {
            var targetCmid = Int32Proxy.Deserialize(bytes);
            var force = Vector3Proxy.Deserialize(bytes);

            Enqueue(() => OnHitFeedback(peer.Actor, targetCmid, force));
        }
    }
}
