using log4net;
using System;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using System.Collections.Generic;
using MoreLinq;

namespace UberStrok.Realtime.Server.Game
{
    public class TeamDeathMatchGameRoom : BaseGameRoom
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(TeamDeathMatchGameRoom));

        public TeamDeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            if (data.GameMode != GameModeType.TeamDeathMatch)
                throw new ArgumentException("GameRoomDataView is not in team deathmatch mode.", nameof(data));
        }

        public int BlueTeamScore { get; set; }
        public int RedTeamScore { get; set; }

        protected override void OnMatchEnded(EventArgs args)
        {
            foreach (var player in Players)
                player.Events.Game.SendUpdateRoundScore(RoundNumber, (short)BlueTeamScore, (short)RedTeamScore);

            if (BlueTeamScore > RedTeamScore)
                EndMatch(TeamID.BLUE);
            else if (RedTeamScore > BlueTeamScore)
                EndMatch(TeamID.RED);
            else if (RedTeamScore == BlueTeamScore)
                EndMatch(TeamID.NONE);

            base.OnMatchEnded(args);
        }

        public void EndMatch(TeamID winningTeam)
        {
            foreach (var player in Players)
            {
                bool hasWon = winningTeam == player.Actor.Team;

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

            if (IsRunning)
            {
                /* This is to reset the top scoreboard to not display "STARTS IN". */
                args.Player.Events.Game.SendUpdateRoundScore(RoundNumber, (short)BlueTeamScore, (short)RedTeamScore);
            }
        }

        protected override void OnPlayerKilled(PlayerKilledEventArgs args)
        {
            base.OnPlayerKilled(args);

            foreach (var player in Players)
            {
                if (player.Actor.Cmid != args.AttackerCmid)
                    continue;

                var killReq = player.Room.View.KillLimit;
                if (player.Actor.Team == TeamID.BLUE)
                {
                    // if this is true, they killed themselves
                    if (args.AttackerCmid == args.VictimCmid)
                        BlueTeamScore--;
                    else
                        BlueTeamScore++;

                    if (BlueTeamScore >= killReq)
                    {
                        OnMatchEnded(new EventArgs());
                        // no need to keep going, also to prevent errors
                        break;
                    }

                }
                else if (player.Actor.Team == TeamID.RED)
                {
                    // if this is true, they killed themselves
                    if (args.AttackerCmid == args.VictimCmid)
                        RedTeamScore--;
                    else
                        RedTeamScore++;
                    RedTeamScore++;
                    if (RedTeamScore >= killReq)
                    {
                        OnMatchEnded(new EventArgs());
                        // no need to keep going, also to prevent errors
                        break;
                    }
                }
            }

            foreach (var player in Players)
                player.Events.Game.SendUpdateRoundScore(RoundNumber, (short)BlueTeamScore, (short)RedTeamScore);
        }
    }
}
