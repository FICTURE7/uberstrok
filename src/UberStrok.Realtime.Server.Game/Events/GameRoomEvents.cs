using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Common;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomEvents : EventSender
    {
        public GameRoomEvents(GamePeer peer) 
            : base(peer)
        {
            /* Space */
        }

        public void SendPowerUpPicked(int pickupId, byte flag)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, pickupId);
                ByteProxy.Serialize(bytes, flag);

                SendEvent((byte)IGameRoomEventsType.PowerUpPicked, bytes);
            }
        }

        public void SendSetPowerUpState(List<int> states)
        {
            using (var bytes = new MemoryStream())
            {
                ListProxy<int>.Serialize(bytes, states, Int32Proxy.Serialize);
                SendEvent((byte)IGameRoomEventsType.SetPowerupState, bytes);
            }
        }

        public void SendResetAllPowerUps()
        {
            using (var bytes = new MemoryStream())
                SendEvent((byte)IGameRoomEventsType.ResetAllPowerups, bytes);
        }

        public void SendPlayerHit(Vector3 force)
        {
            using (var bytes = new MemoryStream())
            {
                Vector3Proxy.Serialize(bytes, force);
                SendEvent((byte)IGameRoomEventsType.PlayerHit, bytes);
            }
        }

        public void SendEmitProjectile(int cmid, Vector3 origin, Vector3 direction, byte slot, int projectileId, bool explode)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                Vector3Proxy.Serialize(bytes, origin);
                Vector3Proxy.Serialize(bytes, direction);
                ByteProxy.Serialize(bytes, slot);
                Int32Proxy.Serialize(bytes, projectileId);
                BooleanProxy.Serialize(bytes, explode);

                SendEvent((byte)IGameRoomEventsType.EmitProjectile, bytes);
            }
        }

        public void SendEmitQuickItem(Vector3 origin, Vector3 direction, int itemId, byte playerNumber, int projectileId)
        {
            using (var bytes = new MemoryStream())
            {
                Vector3Proxy.Serialize(bytes, origin);
                Vector3Proxy.Serialize(bytes, direction);
                Int32Proxy.Serialize(bytes, itemId);
                ByteProxy.Serialize(bytes, playerNumber);
                Int32Proxy.Serialize(bytes, projectileId);

                SendEvent((byte)IGameRoomEventsType.EmitQuickItem, bytes);
            }
        }

        public void SendRemoveProjectile(int projectileId, bool explode)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, projectileId);
                BooleanProxy.Serialize(bytes, explode);

                SendEvent((byte)IGameRoomEventsType.RemoveProjectile, bytes);
            }
        }

        public void SendPlayerRespawnCountdown(int countdown)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, countdown);
                SendEvent((byte)IGameRoomEventsType.PlayerRespawnCountdown, bytes);
            }
        }

        public void SendDisconnectCountdown(int countdown)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, countdown);
                SendEvent((byte)IGameRoomEventsType.DisconnectCountdown, bytes);
            }
        }

        public void SendPlayerKilled(int shooter, int target, UberStrikeItemClass weaponClass, 
                    ushort damage, BodyPart bodyPart, Vector3 direction)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, shooter);
                Int32Proxy.Serialize(bytes, target);
                ByteProxy.Serialize(bytes, (byte)weaponClass);
                UInt16Proxy.Serialize(bytes, damage);
                ByteProxy.Serialize(bytes, (byte)bodyPart);
                Vector3Proxy.Serialize(bytes, direction);

                SendEvent((byte)IGameRoomEventsType.PlayerKilled, bytes);
            }
        }

        public void SendDamageEvent(DamageEventView damageEvent)
        {
            using (var bytes = new MemoryStream())
            {
                DamageEventViewProxy.Serialize(bytes, damageEvent);
                SendEvent((byte)IGameRoomEventsType.DamageEvent, bytes);
            }
        }

        public void SendPrepareNextRound()
        {
            using (var bytes = new MemoryStream())
                SendEvent((byte)IGameRoomEventsType.PrepareNextRound, bytes);
        }

        public void SendPlayerJumped(int cmid, Vector3 position)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                Vector3Proxy.Serialize(bytes, position);

                SendEvent((byte)IGameRoomEventsType.PlayerJumped, bytes);
            }
        }

        public void SendAllPlayerPositions(List<PlayerMovement> allPositions, ushort gameframe)
        {
            using (var bytes = new MemoryStream())
            {
                ListProxy<PlayerMovement>.Serialize(bytes, allPositions, PlayerMovementProxy.Serialize);
                UInt16Proxy.Serialize(bytes, gameframe);

                SendEvent((byte)IGameRoomEventsType.AllPlayerPositions, bytes, true);
            }
        }

        public void SendAllPlayerDeltas(List<GameActorInfoDeltaView> allDeltas)
        {
            using (var bytes = new MemoryStream())
            {
                ListProxy<GameActorInfoDeltaView>.Serialize(bytes, allDeltas, GameActorInfoDeltaViewProxy.Serialize);
                SendEvent((byte)IGameRoomEventsType.AllPlayerDeltas, bytes);
            }
        }

        public void SendAllPlayers(List<GameActorInfoView> allPlayers, List<PlayerMovement> allPositions, ushort gameFrame)
        {
            using (var bytes = new MemoryStream())
            {
                ListProxy<GameActorInfoView>.Serialize(bytes, allPlayers, GameActorInfoViewProxy.Serialize);
                ListProxy<PlayerMovement>.Serialize(bytes, allPositions, PlayerMovementProxy.Serialize);
                UInt16Proxy.Serialize(bytes, gameFrame);

                SendEvent((byte)IGameRoomEventsType.AllPlayers, bytes);
            }
        }

        public void SendPlayerLeftGame(int cmid)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                SendEvent((byte)IGameRoomEventsType.PlayerLeftGame, bytes);
            }
        }

        public void SendWaitingForPlayer()
        {
            using (var bytes = new MemoryStream())
                SendEvent((byte)IGameRoomEventsType.WaitingForPlayers, bytes);
        }

        public void SendChatMessage(int cmid, string name, string message, MemberAccessLevel accessLevel, ChatContext context)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                StringProxy.Serialize(bytes, message);
                EnumProxy<MemberAccessLevel>.Serialize(bytes, accessLevel);
                ByteProxy.Serialize(bytes, (byte)context);

                SendEvent((byte)IGameRoomEventsType.ChatMessage, bytes);
            }
        }

        public void SendPlayerJoinedGame(GameActorInfoView actor, PlayerMovement movement)
        {
            using (var bytes = new MemoryStream())
            {
                GameActorInfoViewProxy.Serialize(bytes, actor);
                PlayerMovementProxy.Serialize(bytes, movement);

                SendEvent((byte)IGameRoomEventsType.PlayerJoinedGame, bytes);
            }
        }

        public void SendJoinGameFailed(string message)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, message);
                SendEvent((byte)IGameRoomEventsType.JoinGameFailed, bytes);
            }
        }

        public void SendUpdateRoundScore(int round, short blue, short red)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, round);
                Int16Proxy.Serialize(bytes, blue);
                Int16Proxy.Serialize(bytes, red);

                SendEvent((byte)IGameRoomEventsType.UpdateRoundScore, bytes);
            }
        }

        public void SendKillsRemaining(int killsRemaining, int leaderCmid)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, killsRemaining);
                Int32Proxy.Serialize(bytes, leaderCmid);

                SendEvent((byte)IGameRoomEventsType.KillsRemaining, bytes);
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

        public void SendMatchEnd(EndOfMatchDataView endOfMatch)
        {
            using (var bytes = new MemoryStream())
            {
                EndOfMatchDataViewProxy.Serialize(bytes, endOfMatch);
                SendEvent((byte)IGameRoomEventsType.MatchEnd, bytes);
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

        public void SendSingleBulletFire(int cmid)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                SendEvent((byte)IGameRoomEventsType.SingleBulletFire, bytes);
            }
        }

        public void SendDoorOpen(int doorId)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, doorId);
                SendEvent((byte)IGameRoomEventsType.DoorOpen, bytes);
            }
        }

        public void SendTeamWins(TeamID team)
        {
            using (var bytes = new MemoryStream())
            {
                EnumProxy<TeamID>.Serialize(bytes, team);
                SendEvent((byte)IGameRoomEventsType.TeamWins, bytes);
            }
        }
    }
}
