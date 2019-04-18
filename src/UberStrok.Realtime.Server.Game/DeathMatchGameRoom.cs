using System;
using System.Linq;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class DeathMatchGameRoom : BaseGameRoom
    {
        public DeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            if (data.GameMode != GameModeType.DeathMatch)
                throw new ArgumentException("GameRoomDataView is not in deathmatch mode", nameof(data));
        }

        protected override bool CanDamage(GamePeer victim, GamePeer attacker)
        {
            return true;
        }

        protected override void OnPlayerJoined(PlayerJoinedEventArgs args)
        {
            base.OnPlayerJoined(args);
            args.Player.Events.Game.SendKillsRemaining(GetKillsRemaining(), 0);
        }

        protected override void OnPlayerLeft(PlayerLeftEventArgs args)
        {
            base.OnPlayerLeft(args);
            
            int killsRemaining = GetKillsRemaining();
            foreach (var player in Players)
                player.Events.Game.SendKillsRemaining(killsRemaining, 0);
        }

        protected override void OnPlayerKilled(PlayerKilledEventArgs args)
        {
            base.OnPlayerKilled(args);

            /* If player killed himself, don't update round score. */
            if (args.Attacker == args.Victim)
                return;

            int killsRemaining = GetKillsRemaining();
            foreach (var player in Players)
                player.Events.Game.SendKillsRemaining(killsRemaining, 0);
        }

        private int GetKillsRemaining()
        {
            /* 
             * NOTE: Possible performance gain by avoiding using LINQ but
             * maintain the leader directly through killed events.
             */
            return View.KillLimit - Players.Aggregate((a, b) => a.Actor.Info.Kills > b.Actor.Info.Kills ? a : b).Actor.Info.Kills;
        }
    }
}
