using log4net;
using System;
using UberStrok.Core.Common;
using System.Collections.Generic;
using System.Collections;
using UberStrok.Core.Views;
using MoreLinq;
using System.Linq;

namespace UberStrok.Realtime.Server.Game
{
    public class DeathMatchGameRoom : BaseGameRoom
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(TeamDeathMatchGameRoom));

        public DeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            if (data.GameMode != GameModeType.DeathMatch)
                throw new ArgumentException("GameRoomDataView is not in deathmatch mode.", nameof(data));
        }

        protected override void OnMatchEnded(EventArgs e)
        {
            base.OnMatchEnded(e);

            int winnerCmid = 0;
            int winnerKills = 0;
            foreach (var player in Players)
                if (player.Actor.Info.Kills > winnerKills)
                    winnerCmid = player.Actor.Cmid;

            EndMatch(winnerCmid);
        }

        public void EndMatch(int winnerCmid)
        {
            List<GamePeer> PeersToMurder = new List<GamePeer>();
            foreach (var player in Players)
            {
                PeersToMurder.Add(player);
                bool hasWon = winnerCmid == player.Actor.Cmid;

                // Calculate MVPs.
                List<StatsSummaryView> MostValuablePlayers = new List<StatsSummaryView>();
                foreach (var mvp in Players)
                {
                    Dictionary<byte, ushort> achievements = new Dictionary<byte, ushort>();
                    // Most Valuable (Highest KD)
                    // KD is divided by 10 by the client because you cant send decimals through ushort
                    if (Players.MaxBy(x => x.TotalStats.GetKills()).Actor.Cmid == mvp.Actor.Cmid)
                        achievements.Add(1, (ushort)(mvp.TotalStats.GetKills() / Math.Max(1, mvp.TotalStats.Deaths) * 10));
                    // Most Aggressive (Most Kills Total)
                    if (Players.MaxBy(x => x.TotalStats.GetKills()).Actor.Cmid == mvp.Actor.Cmid)
                        achievements.Add(2, (ushort)mvp.TotalStats.GetKills());
                    // Sharpest Shooter (Most Crits aka Most Headshots and Nutshots)
                    if (Players.MaxBy(x => x.TotalStats.Headshots + x.TotalStats.Nutshots).Actor.Cmid == mvp.Actor.Cmid)
                        achievements.Add(3, (ushort)(mvp.TotalStats.Headshots + mvp.TotalStats.Nutshots));
                    // Most Trigger Happy (Highest Killstreak)
                    if (Players.MaxBy(x => x.TotalStats.ConsecutiveSnipes).Actor.Cmid == mvp.Actor.Cmid)
                        achievements.Add(4, (ushort)(mvp.TotalStats.ConsecutiveSnipes));
                    // Hardest Hitter (Highest Damage Dealt)
                    if (Players.MaxBy(x => x.TotalStats.GetDamageDealt()).Actor.Cmid == mvp.Actor.Cmid)
                        achievements.Add(5, (ushort)mvp.TotalStats.GetDamageDealt());

                    MostValuablePlayers.Add(new StatsSummaryView
                    {
                        Cmid = mvp.Actor.Cmid,
                        Achievements = achievements,
                        Deaths = mvp.TotalStats.Deaths,
                        Kills = mvp.TotalStats.GetKills(),
                        Level = mvp.Actor.Info.Level,
                        Name = mvp.Actor.PlayerName,
                        Team = mvp.Actor.Team
                    });
                }


                EndOfMatchDataView data = player.GetStats(hasWon, View.Guid, MostValuablePlayers);
                player.Events.Game.SendMatchEnd(data);
                //player.State.Set(PeerState.Id.AfterRound);
            }

            State.Set(MatchState.Id.WaitingForPlayers);
        }

        protected override void OnPlayerJoined(PlayerJoinedEventArgs args)
        {
            base.OnPlayerJoined(args);
        }

        protected override void OnPlayerKilled(PlayerKilledEventArgs args)
        {
            base.OnPlayerKilled(args);
            var leader = Players.MaxBy(x => x.Actor.Info.Kills);
            var remaining = leader.Room.View.KillLimit - leader.TotalStats.GetKills();

            foreach (var player in Players)
                player.Events.Game.SendKillsRemaining(remaining, leader.Actor.Cmid);

            if (leader.Actor.Info.Kills >= leader.Room.View.KillLimit)
                EndMatch(leader.Actor.Cmid);

        }
    }
}