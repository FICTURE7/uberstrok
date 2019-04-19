using System;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class TeamDeathMatchGameRoom : BaseGameRoom
    {
        public bool FriendlyFire { get; set; }
        public int BlueTeamScore { get; private set; }
        public int RedTeamScore { get; private set; }

        public TeamDeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            if (data.GameMode != GameModeType.TeamDeathMatch)
                throw new ArgumentException("GameRoomDataView is not in team deathmatch mode", nameof(data));

            FriendlyFire = false;
        }

        protected override bool CanDamage(GamePeer victim, GamePeer attacker)
        {
            if (FriendlyFire)
                return true;

            return victim == attacker || victim.Actor.Team != attacker.Actor.Team;
        }

        protected override void OnPlayerJoined(PlayerJoinedEventArgs args)
        {
            base.OnPlayerJoined(args);

            /* This is to reset the top scoreboard to not display "STARTS IN". */
            if (IsRunning)
                args.Player.Events.Game.SendUpdateRoundScore(RoundNumber, (short)BlueTeamScore, (short)RedTeamScore);
        }

        protected override void OnPlayerKilled(PlayerKilledEventArgs args)
        {
            base.OnPlayerKilled(args);

            /* If player killed himself, don't update round score. */
            if (args.Attacker == args.Victim)
                return;

            if (args.Attacker.Actor.Team == TeamID.BLUE)
                BlueTeamScore++;
            else if (args.Attacker.Actor.Team == TeamID.RED)
                RedTeamScore++;

            foreach (var player in Players)
                player.Events.Game.SendUpdateRoundScore(RoundNumber, (short)BlueTeamScore, (short)RedTeamScore);
        }
    }
}
