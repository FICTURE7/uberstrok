using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class RunningMatchState : MatchState
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(RunningMatchState));

        /* Current tick we're in. */
        private ushort _frame = 0;

        public RunningMatchState(BaseGameRoom room) : base(room)
        {
            // Space
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

        public override void OnResume()
        {
            // Space
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
            Room.PlayerRespawned -= OnPlayerRespawned;
            Room.PlayerKilled -= OnPlayerKilled;
        }

        private TimeSpan depleteTime;
        public override void OnUpdate()
        {
            /* Expected interval between ticks by the client (10tick/s). */
            const int UBZ_INTERVAL = 100;

            /* Tick the power-up manager. */
            Room.PowerUps.Update();

            var deltas = new List<GameActorInfoDeltaView>(Room.Peers.Count);
            var position = new List<PlayerMovement>(Room.Players.Count);
            
            foreach (var player in Room.Players)
            {
                position.Add(player.Actor.Movement);

                // Armor depletion if over capacity
                if (player.Actor.Info.ArmorPoints > player.Actor.Info.ArmorPointCapacity
                    && DateTime.Now.TimeOfDay >= depleteTime)
                    player.Actor.Info.ArmorPoints--;

                // Maybe let user set max health before depletion?
                // Health depletion if over 100
                if (player.Actor.Info.Health > 100
                    && DateTime.Now.TimeOfDay >= depleteTime)
                    player.Actor.Info.Health--;

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

                /* Tick the player state. */
                player.State.Update();
            }

            /* Send movement and deltas data to all connected peers, including peers in 'overview' state. */
            foreach (var otherPeer in Room.Peers)
            {
                otherPeer.Events.Game.SendAllPlayerDeltas(deltas);
                otherPeer.Events.Game.SendAllPlayerPositions(position, _frame);
            }

            /* Wipe the delta changes. */
            foreach (var delta in deltas)
                delta.Changes.Clear();

            /* Check if players have done single shots. */
            foreach (var player in Room.Players)
            {
                if (player.Actor.Info.ShootingTick > 0)
                {
                    player.Actor.Info.ShootingTick -= 1;

                    if (player.Actor.Info.ShootingTick < 0)
                    {
                        player.Actor.Info.ShootingTick = 0;
                        player.Actor.Info.PlayerState &= ~PlayerStates.Shooting;
                    }
                }
            }

            // Tick armor/hp every second.
            if (DateTime.Now.TimeOfDay >= depleteTime)
                depleteTime = DateTime.Now.TimeOfDay.Add(TimeSpan.FromSeconds(1));

            _frame += (ushort)(UBZ_INTERVAL / Room.Loop.Interval);
        }

        private void OnPlayerKilled(object sender, PlayerKilledEventArgs e)
        {
            /* Let all peers know that the player has died. */
            foreach (var otherPeer in Room.Peers)
                otherPeer.Events.Game.SendPlayerKilled(e.AttackerCmid, e.VictimCmid, e.ItemClass, e.Damage, e.Part, e.Direction);
        }

        private void OnPlayerRespawned(object sender, PlayerRespawnedEventArgs e)
        {
            /* Let all peers know that the player has respawned. */
            e.Player.Actor.Info.Health = 100;
            e.Player.Actor.Info.PlayerState &= ~PlayerStates.Dead;

            // Calculate armor capacity
            foreach (var armor in e.Player.Actor.Info.Gear)
            {
                var gear = default(UberStrikeItemGearView);
                if (Room.ShopManager.GearItems.TryGetValue(armor, out gear))
                    e.Player.Actor.Info.ArmorPointCapacity = (byte)Math.Min(200, e.Player.Actor.Info.ArmorPointCapacity + gear.ArmorPoints);
                else
                    s_log.Debug($"Could not find gear with ID {armor}.");
            }
            // Set armor on spawn to the max capacity
            e.Player.Actor.Info.ArmorPoints = e.Player.Actor.Info.ArmorPointCapacity;

            var spawn = Room.SpawnManager.Get(e.Player.Actor.Team);
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
            var point = Room.SpawnManager.Get(player.Actor.Team);
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
