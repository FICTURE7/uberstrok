using System;
using System.Linq;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class DeathMatchGameRoom : GameRoom
    {
        public override bool CanStart => Players.Count > 1;

        public DeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            if (data.GameMode != GameModeType.DeathMatch)
                throw new ArgumentException("GameRoomDataView is not in deathmatch mode", nameof(data));
        }

        protected override bool CanDamage(GameActor victim, GameActor attacker)
        {
            return true;
        }

        protected override void OnPlayerJoined(PlayerJoinedEventArgs e)
        {
            base.OnPlayerJoined(e);
            e.Player.Peer.Events.Game.SendKillsRemaining(GetKillsRemaining(), 0);
        }

        protected override void OnPlayerLeft(PlayerLeftEventArgs e)
        {
            base.OnPlayerLeft(e);

            if (Players.Count <= 1)
            {
                State.Set(MatchState.Id.End);
            }
            else
            {
                int killsRemaining = GetKillsRemaining();
                foreach (var otherActor in Actors)
                    otherActor.Peer.Events.Game.SendKillsRemaining(killsRemaining, 0);
            }
        }

        protected override void OnPlayerKilled(PlayerKilledEventArgs e)
        {
            base.OnPlayerKilled(e);

            /* If player killed himself, don't update round score. */
            if (e.Attacker == e.Victim)
                return;

            int killsRemaining = GetKillsRemaining();
            foreach (var player in Players)
                player.Peer.Events.Game.SendKillsRemaining(killsRemaining, 0);

            /* 
             * The client doesn't care about which team wins, it uses
             * its local data to figure out which player won.
             */
            if (killsRemaining <= 0)
                State.Set(MatchState.Id.End);
        }

        private int GetKillsRemaining()
        {
            /* 
             * NOTE: Possible performance gain by avoiding using LINQ but
             * maintain the leader directly through killed events.
             */
            return GetView().KillLimit - Players.Aggregate((a, b) => a.Info.Kills > b.Info.Kills ? a : b).Info.Kills;
        }
    }
}
