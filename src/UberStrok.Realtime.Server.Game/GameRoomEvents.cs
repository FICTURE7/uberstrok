using System.Collections.Generic;
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

        public void SendPlayerKilled(int shooter, int target, UberstrikeItemClass weaponClass, ushort damage, BodyPart bodyPart, Vector3 direction)
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

                SendEvent((byte)IGameRoomEventsType.AllPlayerPositions, bytes);
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
