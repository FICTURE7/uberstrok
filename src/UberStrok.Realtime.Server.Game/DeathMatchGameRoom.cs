using log4net;
using System;
using System.Linq;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class DeathMatchGameRoom : BaseGameRoom
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(DeathMatchGameRoom));

        public DeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            if (data.GameMode != GameModeType.DeathMatch)
                throw new ArgumentException("GameRoomDataView is not in deathmatch mode.", nameof(data));
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
            if (args.AttackerCmid == args.VictimCmid)
                return;

            int killsRemaining = GetKillsRemaining();

            foreach (var player in Players)
                player.Events.Game.SendKillsRemaining(killsRemaining, 0);
        }

        private int GetKillsRemaining()
        {
            return View.KillLimit - Players.Aggregate((a, b) => a.Actor.Info.Kills > b.Actor.Info.Kills ? a : b).Actor.Info.Kills;
        }
    }
}
