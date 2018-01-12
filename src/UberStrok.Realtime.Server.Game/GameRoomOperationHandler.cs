using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomOperationHandler : BaseGameRoomOperationHandler
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(GameRoomOperationHandler));
        private readonly BaseGameRoom _room;

        public GameRoomOperationHandler(BaseGameRoom room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            _room = room;
        }

        public override int Id => 0;
        protected BaseGameRoom Room => _room;

        protected override void OnChatMessage(GamePeer peer, string message, ChatContext context)
        {
            var cmid = peer.Member.CmuneMemberView.PublicProfile.Cmid;
            var name = peer.Member.CmuneMemberView.PublicProfile.Name;
            var accessLevel = peer.Member.CmuneMemberView.PublicProfile.AccessLevel;

            foreach (var opeer in Room.Peers)
            {
                if (opeer.Member.CmuneMemberView.PublicProfile.Cmid != cmid)
                    opeer.Events.Game.SendChatMessage(cmid, name, message, accessLevel, context);
            }
        }

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

            foreach (var opeer in Room.Peers)
                opeer.Events.Game.SendPlayerJoinGame(player, new PlayerMovement());

            s_log.Info($"Joining team -> CMID:{player.Cmid}:{team}");

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
        }

        protected override void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes)
        {
            var times = string.Join(", ", respawnTimes);

            s_log.Info($"Respawn Times -> {times}");
        }

        protected override void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Team: {team}");

            for (int i = 0; i < positions.Count; i++)
                builder.AppendLine($" - {positions[i]}: {rotations[i]}");

            /*
            _respawns = positions;
            _respawnRotations = rotations;
            */

            s_log.Info($"Spawn Points -> {builder}");
        }
    }
}
