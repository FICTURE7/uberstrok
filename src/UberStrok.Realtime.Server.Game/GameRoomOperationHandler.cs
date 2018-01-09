using log4net;
using System.Collections.Generic;
using System.Text;
using UberStrok.Core.Common;
using System;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomOperationHandler : BaseGameRoomOperationHandler
    {
        public GameRoomOperationHandler(BaseGameRoom room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            _room = room;
        }
        
        public override int Id => 0;

        protected BaseGameRoom Room => _room;

        private List<Vector3> _respawns;
        private List<byte> _respawnRotations;
        private readonly BaseGameRoom _room;

        protected override void OnJoinTeam(GamePeer peer, TeamID team)
        {
            var player = new GameActorInfoView
            {
                TeamID = team,
                Health = 100,
                PlayerId = 1,
                Channel = ChannelType.Steam,
                PlayerState = PlayerStates.None,

                Cmid = peer.Member.CmuneMemberView.PublicProfile.Cmid,
                ClanTag = peer.Member.CmuneMemberView.PublicProfile.GroupTag,
                AccessLevel = peer.Member.CmuneMemberView.PublicProfile.AccessLevel,
                Ping = (ushort)(peer.RoundTripTime / 2),
                PlayerName = peer.Member.CmuneMemberView.PublicProfile.Name,
                Weapons = peer.Member.CmuneMemberView.MemberItems,
            };

            LogManager.GetLogger(typeof(GameRoomOperationHandler)).Info($"Joining team -> CMID:{player.Cmid}:{team}");

            int index = new Random().Next(_respawnRotations.Count);
            peer.Events.Game.SendPlayerJoinGame(player, new PlayerMovement());
            peer.Events.Game.SendMatchStart(0, peer.Room.Data.TimeLimit);
            peer.Events.Game.SendPlayerRespawned(player.Cmid, _respawns[index], _respawnRotations[index]);

            /*
            GameApplication.Instance.Scheduler.Add(() =>
            {
                Peer.Game.Events.SendMatchStartCountdown(1);
            }, DateTime.UtcNow.AddSeconds(3));
            GameApplication.Instance.Scheduler.Add(() =>
            {
                Peer.Game.Events.SendMatchStartCountdown(2);
            }, DateTime.UtcNow.AddSeconds(2));
            GameApplication.Instance.Scheduler.Add(() =>
            {
                Peer.Game.Events.SendMatchStartCountdown(3);
            }, DateTime.UtcNow.AddSeconds(1));

            GameApplication.Instance.Scheduler.Add(() =>
            {
                //Peer.Game.Events.SendMatchStart(0, Peer.Game.Room.Data.TimeLimit);
            }, DateTime.UtcNow.AddSeconds(4));
            */

            /*
            Peer.Game.Events.SendPlayerJoinGame(player, new PlayerMovement());
            Peer.Game.Events.SendPlayerJoinGame(player, new PlayerMovement());
            Peer.Game.Events.SendPlayerJoinGame(player, new PlayerMovement());
            Peer.Game.Events.SendPlayerJoinGame(player, new PlayerMovement());
            Peer.Game.Events.SendPlayerJoinGame(player, new PlayerMovement());
            */
        }

        protected override void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes)
        {
            var times = string.Join(", ", respawnTimes);

            LogManager.GetLogger(typeof(GameRoomOperationHandler)).Info($"Respawn Times -> {times}");
        }

        protected override void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Team: {team}");

            for (int i = 0; i < positions.Count; i++)
                builder.AppendLine($" - {positions[i]}: {rotations[i]}");

            _respawns = positions;
            _respawnRotations = rotations;

            LogManager.GetLogger(typeof(GameRoomOperationHandler)).Info($"Spawn Points -> {builder}");
        }
    }
}
