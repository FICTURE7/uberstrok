using System;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class TeamDeathMatchGameRoom : GameRoom
    {
        public override bool CanStart => BlueTeamPlayer > 0 && RedTeamPlayer > 0;

        public bool FriendlyFire { get; set; }
        public int BlueTeamScore { get; private set; }
        public int RedTeamScore { get; private set; }

        public int BlueTeamPlayer { get; private set; }
        public int RedTeamPlayer { get; private set; }

        public TeamDeathMatchGameRoom(GameRoomDataView data, ILoopScheduler scheduler) 
            : base(data, scheduler)
        {
            if (data.GameMode != GameModeType.TeamDeathMatch)
                throw new ArgumentException("GameRoomDataView is not in team deathmatch mode", nameof(data));

            FriendlyFire = false;
        }

        public override void Reset()
        {
            base.Reset();

            BlueTeamPlayer = 0;
            BlueTeamScore = 0;
            RedTeamPlayer = 0;
            RedTeamScore = 0;
        }

        protected sealed override bool CanDamage(GameActor victim, GameActor attacker)
        {
            if (FriendlyFire)
                return true;

            return victim == attacker || victim.Info.TeamID != attacker.Info.TeamID;
        }

        protected sealed override void OnPlayerJoined(PlayerJoinedEventArgs args)
        {
            if (args.Team == TeamID.BLUE)
                BlueTeamPlayer++;
            else if (args.Team == TeamID.RED)
                RedTeamPlayer++;

            base.OnPlayerJoined(args);

            /* This is to reset the top scoreboard to not display "STARTS IN". */
            if (State.Current == MatchState.Id.Running)
            {
                args.Player.Peer.Events.Game.SendUpdateRoundScore(
                    RoundNumber,
                    (short)BlueTeamScore,
                    (short)RedTeamScore
                );
            }
        }

        protected sealed override void OnPlayerLeft(PlayerLeftEventArgs args)
        {
            if (args.Player.Info.TeamID == TeamID.BLUE)
                BlueTeamPlayer--;
            else if (args.Player.Info.TeamID == TeamID.RED)
                RedTeamPlayer--;

            base.OnPlayerLeft(args);

            if (State.Current != MatchState.Id.WaitingForPlayers && (RedTeamPlayer == 0 || BlueTeamPlayer == 0))
                State.Set(MatchState.Id.End);
        }

        protected sealed override void OnPlayerKilled(PlayerKilledEventArgs args)
        {
            base.OnPlayerKilled(args);

            /* If player killed himself, don't update round score. */
            if (args.Attacker == args.Victim)
                return;

            if (args.Attacker.Info.TeamID == TeamID.BLUE)
                BlueTeamScore++;
            else if (args.Attacker.Info.TeamID == TeamID.RED)
                RedTeamScore++;

            foreach (var otherActor in Actors)
            {
                otherActor.Peer.Events.Game.SendUpdateRoundScore(
                    RoundNumber, 
                    (short)BlueTeamScore, 
                    (short)RedTeamScore
                );
            }

            if (BlueTeamScore == RedTeamScore)
                Winner = TeamID.NONE;
            else if (BlueTeamScore > RedTeamScore)
                Winner = TeamID.BLUE;
            else
                Winner = TeamID.RED;

            if (BlueTeamScore >= GetView().KillLimit || RedTeamScore >= GetView().KillLimit)
                State.Set(MatchState.Id.End);
        }
    }
}
