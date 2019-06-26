using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class RunningMatchState : MatchState
    {
        /* 
         * Current tick we're in.
         * 
         * NOTE: _frame starts at 5 because the client resets the lag 
         * interpolator's base values.
         */
        private ushort _frame = 5;
        private double _frameTime = 0f;

        public RunningMatchState(BaseGameRoom room) : base(room)
        {
            /* Space */
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;
            Room.PlayerRespawned += OnPlayerRespawned;
            Room.PlayerKilled += OnPlayerKilled;

            /* Calculate the time when the games ends (in system ticks). */
            Room.EndTime = Environment.TickCount + Room.View.TimeLimit * 1000;

            foreach (var player in Room.Players)
                player.State.Set(PeerState.Id.Playing);

            /* TODO: Increment round number only when the round is over. */
            Room.RoundNumber++;
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
            Room.PlayerRespawned -= OnPlayerRespawned;
            Room.PlayerKilled -= OnPlayerKilled;
        }

        public override void OnUpdate()
        {
            /* 
             * Expected interval between ticks by the client is 100ms (10tick/s),
             * and since the implementation of Loop is a fixed timestamp 
             * implementation that tries to catch up with sleep lag, we use 
             * Loop.DeltaTime to calculate real time elapsed to send player 
             * positions at regular intervals. That is intervals of around 100ms.
             *
             * Lag extrapolation starts when the packets arrive at around 150ms
             * late.
             */
            const int UBZ_INTERVAL = 125;

            _frameTime += Room.Loop.DeltaTime.TotalMilliseconds;

            bool updatePositions = _frameTime >= UBZ_INTERVAL;
            if (updatePositions)
            {
                _frameTime %= UBZ_INTERVAL;
                _frame++;
            }

            /* Tick the power-up manager. */
            Room.PowerUps.Update();

            var deltas = new List<GameActorInfoDeltaView>(Room.Peers.Count);
            var position = updatePositions ? new List<PlayerMovement>(Room.Players.Count) : null;
            foreach (var player in Room.Players)
            {
                if (updatePositions && (player.Actor.Info.PlayerState & PlayerStates.Dead) != PlayerStates.Dead)
                    position.Add(player.Actor.Movement);

                /* If the player has any damage events, we sent them. */
                if (player.Actor.Damages.Count > 0)
                {
                    player.Events.Game.SendDamageEvent(player.Actor.Damages);
                    player.Actor.Damages.Clear();
                }

                /* If the player has changed something since the last tick. */
                var delta = player.Actor.Info.ViewDelta;
                if (delta.Changes.Count > 0)
                {
                    delta.UpdateMask();
                    deltas.Add(delta);
                }
            }

            /* Send movement and deltas data to all connected peers, including peers in 'overview' state. */
            foreach (var otherPeer in Room.Peers)
            {
                otherPeer.Events.Game.SendAllPlayerDeltas(deltas);
                if (updatePositions)
                    otherPeer.Events.Game.SendAllPlayerPositions(position, _frame);

                /* Tick the player state. */
                otherPeer.State.Update();
            }

            /* Wipe the delta changes. */
            foreach (var delta in deltas)
                delta.Changes.Clear();
        }

        private void OnPlayerKilled(object sender, PlayerKilledEventArgs e)
        {
            /* Let all peers know that the player has died. */
            foreach (var otherPeer in Room.Peers)
                otherPeer.Events.Game.SendPlayerKilled(e.Attacker.Actor.Cmid, e.Victim.Actor.Cmid, e.ItemClass, e.Damage, e.Part, e.Direction);
        }

        private void OnPlayerRespawned(object sender, PlayerRespawnedEventArgs e)
        {
            /* Let all peers know that the player has respawned. */
            e.Player.Actor.Info.Health = 100;
            e.Player.Actor.Info.ArmorPoints = e.Player.Actor.Info.ArmorPointCapacity;
            e.Player.Actor.Info.PlayerState &= ~PlayerStates.Dead;

            var spawn = Room.Spawns.Get(e.Player.Actor.Team);
            foreach (var otherPeer in Room.Peers)
                otherPeer.Events.Game.SendPlayerRespawned(e.Player.Actor.Cmid, spawn.Position, spawn.Rotation);

            /* Switch to previous state which should be 'playing state'. */
            e.Player.State.Previous();

            Debug.Assert(e.Player.State.Current == PeerState.Id.Playing);
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            /* Spawn the player in the map. */
            var player = e.Player;
            var point = Room.Spawns.Get(player.Actor.Team);
            player.Actor.Movement.Position = point.Position;
            player.Actor.Movement.HorizontalRotation = point.Rotation;

            player.Events.Game.SendMatchStart(Room.RoundNumber, Room.EndTime);

            /* Let all peers know that the client has joined & has spawned. */
            foreach (var otherPeer in Room.Peers)
            {
                Debug.Assert(!otherPeer.KnownActors.Contains(player.Actor.Cmid));

                otherPeer.Events.Game.SendPlayerJoinedGame(player.Actor.Info.View, player.Actor.Movement);
                otherPeer.Events.Game.SendPlayerRespawned(player.Actor.Cmid, point.Position, point.Rotation);

                otherPeer.KnownActors.Add(player.Actor.Cmid);
            }

            /* Sync the power ups to the server side. */
            player.Events.Game.SendSetPowerUpState(Room.PowerUps.Respawning);

            player.State.Set(PeerState.Id.Playing);
        }
    }
}
