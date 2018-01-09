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
        public GameRoomOperationHandler(GamePeer peer) : base(peer)
        {
            // Space
        }

        public override int Id => 0;

        private List<Vector3> _respawns;
        private List<byte> _respawnRotations;

        protected override void OnJoinTeam(TeamID team)
        {
            var player = new GameActorInfoView
            {
                Cmid = Peer.Member.CmuneMemberView.PublicProfile.Cmid,
                TeamID = team,
                ClanTag = Peer.Member.CmuneMemberView.PublicProfile.GroupTag,
                Channel = ChannelType.Steam,
                AccessLevel = Peer.Member.CmuneMemberView.PublicProfile.AccessLevel,
                Health = 100,
                Ping = (ushort)(Peer.RoundTripTime / 2),
                PlayerName = Peer.Member.CmuneMemberView.PublicProfile.Name,
                PlayerId = 1,
                Weapons = Peer.Member.CmuneMemberView.MemberItems,
                PlayerState = PlayerStates.Ready
            };

            LogManager.GetLogger(typeof(GameRoomOperationHandler)).Info($"Joining team -> CMID:{player.Cmid}:{team}");

            int index = new Random().Next(_respawnRotations.Count);
            Peer.Game.Events.SendPlayerJoinGame(player, new PlayerMovement());
            Peer.Game.Events.SendMatchStart(0, Peer.Game.Room.Data.TimeLimit);
            Peer.Game.Events.SendPlayerRespawned(player.Cmid, _respawns[index], _respawnRotations[index]);
            
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

        protected override void OnPowerUpRespawnTimes(List<ushort> respawnTimes)
        {
            var times = string.Join(", ", respawnTimes);

            LogManager.GetLogger(typeof(GameRoomOperationHandler)).Info($"Respawn Times -> {times}");
        }

        protected override void OnSpawnPositions(TeamID team, List<Vector3> positions, List<byte> rotations)
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
