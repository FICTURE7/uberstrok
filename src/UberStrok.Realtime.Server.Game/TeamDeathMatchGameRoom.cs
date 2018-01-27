using log4net;
using System;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.Realtime.Server.Game.Core;

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

                if (player.Actor.Team == TeamID.BLUE)
                    BlueTeamScore++;
                else if (player.Actor.Team == TeamID.RED)
                    RedTeamScore++;
            }

            foreach (var player in Players)
                player.Events.Game.SendUpdateRoundScore(RoundNumber, (short)BlueTeamScore, (short)RedTeamScore);
        }
    }
}
