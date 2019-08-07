using System;
using System.Linq;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class DeathMatchGameRoom : GameRoom
    {
        public DeathMatchGameRoom(GameRoomDataView data, ILoopScheduler scheduler) 
            : base(data, scheduler)
        {
            if (data.GameMode != GameModeType.DeathMatch)
                throw new ArgumentException("GameRoomDataView is not in deathmatch mode", nameof(data));
        }

        public override bool CanStart()
        {
            return Players.Count > 1;
        }

        public override bool CanJoin(GameActor actor, TeamID team)
        {
            if (actor.Info.AccessLevel >= MemberAccessLevel.Moderator)
                return true;
            return team == TeamID.NONE && !GetView().IsFull;
        }

        public override bool CanDamage(GameActor victim, GameActor attacker)
        {
            return true;
        }

        protected override void OnPlayerJoined(PlayerJoinedEventArgs e)
        {
            base.OnPlayerJoined(e);
            e.Player.Peer.Events.Game.SendKillsRemaining(GetKillsRemaining(), default);
        }

        protected override void OnPlayerLeft(PlayerLeftEventArgs e)
        {
            base.OnPlayerLeft(e);

            if (State.Current != RoomState.Id.WaitingForPlayers && Players.Count <= 1)
            {
                State.Set(RoomState.Id.End);
            }
            else if (State.Current == RoomState.Id.Running)
            {
                int killsRemaining = GetKillsRemaining();
                foreach (var otherActor in Actors)
                    otherActor.Peer.Events.Game.SendKillsRemaining(killsRemaining, default);
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
                State.Set(RoomState.Id.End);
        }

        private int GetKillsRemaining()
        {
            /* 
             * NOTE: Possible performance gain by avoiding using LINQ but
             * maintain the leader directly through killed events.
             */
            return GetView().KillLimit - (Players.Count > 0 ? Players.Aggregate((a, b) => a.Info.Kills > b.Info.Kills ? a : b).Info.Kills : 0);
        }
    }
}
