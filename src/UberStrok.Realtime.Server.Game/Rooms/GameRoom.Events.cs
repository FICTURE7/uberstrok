using System;
using System.Diagnostics;
using UberStrok.Core;

namespace UberStrok.Realtime.Server.Game
{
    public abstract partial class GameRoom
    {
        public event EventHandler<PlayerKilledEventArgs> PlayerKilled;
        public event EventHandler<PlayerRespawnedEventArgs> PlayerRespawned;
        public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;
        public event EventHandler<PlayerLeftEventArgs> PlayerLeft;

        protected virtual void OnPlayerRespawned(PlayerRespawnedEventArgs args)
        {
            if (args.Player.State.Current != ActorState.Id.Killed)
            {
                Log.Error($"{args.Player.GetDebug()} failed to respawned was not in killed state.");
            }
            else
            {
                Spawn(args.Player);
                args.Player.State.Previous();

                PlayerRespawned?.Invoke(this, args);
            }
        }

        protected virtual void OnPlayerKilled(PlayerKilledEventArgs args)
        {
            args.Victim.State.Set(ActorState.Id.Killed);

            /* Let all actors know that the player has died. */
            foreach (var otherActor in Actors)
            {
                otherActor.Peer.Events.Game.SendPlayerKilled(
                    args.Attacker.Cmid, 
                    args.Victim.Cmid, 
                    args.ItemClass, 
                    args.Damage, 
                    args.Part, 
                    args.Direction
                );
            }

            Log.Info($"{args.Victim.GetDebug()} was killed by {args.Attacker.GetDebug()}.");
            PlayerKilled?.Invoke(this, args);
        }

        protected virtual void OnPlayerJoined(PlayerJoinedEventArgs args)
        {
            /* Try to get the player's old stats in this room, if he had one. */
            if (_stats.TryGetValue(args.Player.Cmid, out StatisticsManager statistics))
                args.Player.Statistics = statistics;
            else
                _stats.Add(args.Player.Cmid, args.Player.Statistics);

            _players.Add(args.Player);

            /* Let the actors know the player has joined the room. */
            foreach (var otherActor in Actors)
            {
                Debug.Assert(otherActor.Movement.PlayerId == otherActor.PlayerId);

                otherActor.Peer.Events.Game.SendPlayerJoinedGame(
                    args.Player.Info.GetView(), 
                    args.Player.Movement
                );
            }

            Log.Info($"{args.Player.GetDebug()} joined game in team {args.Team}.");
            PlayerJoined?.Invoke(this, args);
        }

        protected virtual void OnPlayerLeft(PlayerLeftEventArgs args)
        {
            if (_players.Remove(args.Player))
            {
                /* Let other actors know that the player has left the room. */
                foreach (var otherActor in Actors)
                    otherActor.Peer.Events.Game.SendPlayerLeftGame(args.Player.Cmid);

                args.Player.Peer.GetStats(out int rtt, out int rttVar, out int numFailures);
                Log.Info($"{args.Player.GetDebug()} left game. RTT: {rtt} var<RTT>: {rttVar} NumFailures: {numFailures}");
                PlayerLeft?.Invoke(this, args);
            }
        }
    }
}
