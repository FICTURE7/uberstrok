using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public abstract partial class GameRoom : IRoom<GamePeer>, IDisposable
    {
        private bool _disposed;

        private ushort _frame;
        private double _frameTime;

        private byte _nextPlayer;
        private string _password;

        private readonly GameRoomDataView _view;

        /* 
         * Dictionary mapping player CMIDs to StatisticsManager instances.
         * This is used for when a player leaves and joins the game again; so
         * as to retain his stats.
         */
        private readonly Dictionary<int, StatisticsManager> _stats;
        /* List of cached player stats for end game. */
        private List<StatsSummaryView> _mvps;

        /* List of actor info delta. */
        private readonly List<GameActorInfoDeltaView> _actorDeltas;
        /* List of actor movement. */
        private readonly List<PlayerMovement> _actorMovements;

        private readonly List<GameActor> _actors;
        private readonly List<GameActor> _players;

        protected ILog Log { get; }
        protected ILog ReportLog { get; }

        public Loop Loop { get; }

        public ICollection<GameActor> Players { get; }
        public ICollection<GameActor> Actors { get; }

        public StateMachine<MatchState.Id> State { get; }

        public ShopManager Shop { get; }
        public SpawnManager Spawns { get; }
        public PowerUpManager PowerUps { get; }

        public int RoundNumber { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }

        public TeamID Winner { get; protected set; }
        public abstract bool CanStart { get; }

        /* 
         * Room ID but we call it number since we already defined Id &
         * thats how UberStrike calls it too. 
         */
        public int RoomId
        {
            get => _view.RoomId;
            set => _view.RoomId = value;
        }

        public string Password
        {
            get => _password;
            set
            {
                /* 
                 * If the password is null or empty it means its not
                 * password protected. 
                 */
                _view.IsPasswordProtected = !string.IsNullOrEmpty(value);
                _password = _view.IsPasswordProtected ? value : null;
            }
        }

        public GameRoom(GameRoomDataView data)
        {
            _view = data ?? throw new ArgumentNullException(nameof(data));

            Log = LogManager.GetLogger(GetType().Name);
            ReportLog = LogManager.GetLogger("Report");

            int capacity = data.PlayerLimit / 2;

            _stats = new Dictionary<int, StatisticsManager>();

            _players = new List<GameActor>(capacity);
            _actors = new List<GameActor>(capacity); 
            _actorDeltas = new List<GameActorInfoDeltaView>(capacity);
            _actorMovements = new List<PlayerMovement>(capacity);

            Players = _players;
            Actors = _actors;

            /* 
             * Using a high tick rate to push updates to the client faster.
             * But the player movements are still sent at 10 tps.
             */
            Loop = new Loop(64);

            Shop = new ShopManager();
            Spawns = new SpawnManager();
            PowerUps = new PowerUpManager(this);

            State = new StateMachine<MatchState.Id>();
            State.Register(MatchState.Id.None, null);
            State.Register(MatchState.Id.WaitingForPlayers, new WaitingForPlayersMatchState(this));
            State.Register(MatchState.Id.Countdown, new CountdownMatchState(this));
            State.Register(MatchState.Id.Running, new RunningMatchState(this));
            State.Register(MatchState.Id.End, new EndMatchState(this));

            Reset();

            /* Start the game room loop. */
            Loop.Start(OnTick, OnTickFailed);
        }

        public void Join(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));
            if (peer.Actor != null)
                throw new InvalidOperationException("Peer already in another room");

            if (Loop.IsInLoop)
                DoJoin(peer);
            else
                Enqueue(() => DoJoin(peer));
        }

        public void Leave(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));
            if (peer.Actor == null)
                throw new InvalidOperationException("Peer is not in a room");
            if (peer.Actor.Room != this)
                throw new InvalidOperationException("Peer is not leaving the correct room");

            if (Loop.IsInLoop)
                DoLeave(peer);
            else
                Enqueue(() => DoLeave(peer));
        }

        public void Spawn(GameActor actor)
        {
            if (actor == null)
                throw new ArgumentNullException(nameof(actor));

            if (Loop.IsInLoop)
                DoSpawn(actor);
            else
                Enqueue(() => DoSpawn(actor));
        }

        private struct Achievement
        {
            public Dictionary<AchievementType, Tuple<StatsSummaryView, ushort>> All;
            public AchievementType Type;
            public int Value;

            public Achievement(AchievementType type, Dictionary<AchievementType, Tuple<StatsSummaryView, ushort>> all)
            {
                All = all;
                Type = type;
                Value = int.MinValue;
            }

            public void Check(StatsSummaryView summary, int value)
            {
                if (Value == value)
                {
                    All.Remove(Type);
                }
                else if (value > Value)
                {
                    Value = value;

                    if (Value > 0)
                        All[Type] = new Tuple<StatsSummaryView, ushort>(summary, (ushort)value);
                }
            }
        }

        public List<StatsSummaryView> GetMvps(bool force = false)
        {
            if (_mvps == null || force)
            {
                _mvps = new List<StatsSummaryView>();

                var achievements = new Dictionary<AchievementType, Tuple<StatsSummaryView, ushort>>();
                var mostValuable = new Achievement(AchievementType.MostValuable, achievements);
                var mostAggressive = new Achievement(AchievementType.MostAggressive, achievements);
                var costEffective = new Achievement(AchievementType.CostEffective, achievements);
                var hardestHitter = new Achievement(AchievementType.HardestHitter, achievements);
                var sharpestShooter = new Achievement(AchievementType.SharpestShooter, achievements);
                var triggerHappy = new Achievement(AchievementType.TriggerHappy, achievements);
                
                foreach (var player in Players)
                {
                    var stats = player.Statistics.Total;
                    var summary = new StatsSummaryView
                    {
                        Cmid = player.Cmid,
                        Name = player.PlayerName,
                        Kills = player.Info.Kills,
                        Deaths = player.Info.Deaths,
                        Level = player.Info.Level,  
                        Team = player.Info.TeamID,
                        Achievements = new Dictionary<byte, ushort>()
                    };

                    _mvps.Add(summary);

                    int kills = player.Statistics.Total.GetKills();
                    int deaths = player.Statistics.Total.Deaths;
                    int kdr;

                    if (kills == deaths)
                        kdr = 10;
                    else
                        kdr = (int)Math.Floor((float)kills / Math.Max(1, deaths)) * 10;

                    int shots = player.Statistics.Total.GetShots();
                    int hits = player.Statistics.Total.GetHits();

                    int accuracy = (int)Math.Floor((float)hits / Math.Max(1, shots) * 1000f);
                    int damageDealt = player.Statistics.Total.GetDamageDealt();
                    int criticalHits = player.Statistics.Total.Headshots + player.Statistics.Total.Nutshots;
                    int consecutiveKills = player.Statistics.MostConsecutiveKills;

                    mostValuable.Check(summary, kdr);
                    mostAggressive.Check(summary, kills);
                    costEffective.Check(summary, accuracy);
                    hardestHitter.Check(summary, damageDealt);
                    sharpestShooter.Check(summary, criticalHits);
                    triggerHappy.Check(summary, consecutiveKills);
                }

                foreach (var kv in achievements)
                {
                    var tuple = kv.Value;
                    var achievement = kv.Key;

                    tuple.Item1.Achievements.Add((byte)achievement, tuple.Item2);
                }
            }

            return _mvps;
        }

        public virtual void Reset()
        {
            _frame = 0;
            _frameTime = 0f;
            _nextPlayer = 0;

            /* Reset all the actors in the room's player list. */
            foreach (var player in Players)
            {
                foreach (var otherActor in Actors)
                    otherActor.Peer.Events.Game.SendPlayerLeftGame(player.Cmid);
            }

            _mvps = null;
            _stats.Clear();
            _players.Clear();

            PowerUps.Reset();

            State.Reset();
            State.Set(MatchState.Id.WaitingForPlayers);

            Log.Info($"{GetDebug()} has been reset.");
        }

        private void OnTick()
        {
            State.Tick();
            PowerUps.Tick();

            /* 
             * Expected interval between ticks by the client is 100ms (10tick/s),
             *
             * Lag extrapolation starts when the packets arrive at around 150ms
             * late assuming the client receives a constant stream of packets
             * of 100ms intervals.
             */
            const double UBZ_INTERVAL = 125;

            _frameTime += Loop.DeltaTime.TotalMilliseconds;

            bool updatePositions = _frameTime >= UBZ_INTERVAL;
            if (updatePositions)
            {
                _frameTime %= UBZ_INTERVAL;
                _frame++;
            }

            foreach (var actor in Actors)
            {
                if (actor.Peer.HasError)
                {
                    actor.Peer.Disconnect();
                }
                else
                {
                    try
                    {
                        actor.Tick();
                    }
                    catch (Exception ex)
                    {
                        /* 
                         * NOTE: This should never happen, but just incase 
                         * stuff goes wild.
                         */
                        Log.Error($"Failed to tick {actor.GetDebug()}.", ex);
                        actor.Peer.Disconnect();

                        /* Something happened; we dip. */
                        continue;
                    }

                    var delta = actor.Info.GetViewDelta();

                    /* 
                     * If the actor has changed something since the last tick.
                     */
                    if (delta.Changes.Count > 0)
                    {
                        delta.Update();
                        _actorDeltas.Add(delta);
                    }

                    /* 
                     * If the actor is a player.
                     */
                    if (Players.Contains(actor))
                    {
                        /* 
                         * If the player has any damage events, we send them.
                         */
                        if (actor.Damages.Count > 0)
                        {
                            actor.Peer.Events.Game.SendDamageEvent(actor.Damages);
                            actor.Damages.Clear();
                        }

                        /* 
                         * If we need to update positions and the player is 
                         * alive, we register it to the list of movements.
                         */
                        if (updatePositions && actor.Info.IsAlive)
                        {
                            Debug.Assert(actor.Movement.PlayerId == actor.PlayerId);
                            _actorMovements.Add(actor.Movement);
                        }
                    }
                }
            }

            if (_actorDeltas.Count > 0)
            {
                foreach (var actor in Actors)
                    actor.Peer.Events.Game.SendAllPlayerDeltas(_actorDeltas);

                /* Wipe actor delta changes. */
                foreach (var delta in _actorDeltas)
                    delta.Reset();

                _actorDeltas.Clear();
            }

            if (_actorMovements.Count > 0 && updatePositions)
            {
                foreach (var actor in Actors)
                    actor.Peer.Events.Game.SendAllPlayerPositions(_actorMovements, _frame);

                /* Wipe player movements. */
                _actorMovements.Clear();
            }
        }

        private void OnTickFailed(Exception ex)
        {
            Log.Error("Failed to tick game loop.", ex);
        }

        /* This is executed on the game room loop thread. */
        private void DoJoin(GamePeer peer)
        {
            Debug.Assert(peer != null);

            try
            {
                peer.Handlers.Add(this);

                /* 
                 * This prepares the client for the game room; that is it 
                 * creates the game room instance type and registers the
                 * GameRoom OperationHandler to its photon client.
                 */
                peer.Events.SendRoomEntered(GetView());

                peer.Actor = new GameActor(peer, this);
                peer.Actor.State.Set(ActorState.Id.Overview);
            }
            catch (Exception ex)
            {
                peer.Actor = null;
                peer.Handlers.Remove(Id);

                /* The client doesn't care about `server` and `roomId`. */
                peer.Events.SendRoomEnterFailed(default, default, "Failed to join room.");

                Log.Error($"Failed to join {GetDebug()}.", ex);
                /* Something went wrong; we dip. */
                return;
            }

            _actors.Add(peer.Actor);

            Log.Info($"{peer.Actor.GetDebug()} joined.");
        }

        private void DoLeave(GamePeer peer)
        {
            Debug.Assert(peer != null);
            Debug.Assert(peer.Actor.Room == this);

            var actor = peer.Actor;

            if (_actors.Remove(actor))
            {
                if (_players.Contains(actor)) 
                    OnPlayerLeft(new PlayerLeftEventArgs { Player = peer.Actor });

                /* OnPlayerLeft should remove it. */
                Debug.Assert(!_players.Contains(actor));

                /* Clean up. */
                peer.Actor = null;
                peer.Handlers.Remove(Id);

                Log.Info($"{actor.GetDebug()} left.");
            }
            else
            {
                Log.Warn($"{actor.GetDebug()} tried to leave but was not in the list of Actors.");
            }
        }

        private void DoSpawn(GameActor actor)
        {
            Debug.Assert(actor != null);
            Debug.Assert(actor.Room == this);

            var spawn = Spawns.Get(actor.Info.TeamID);
            var movement = actor.Movement;

            Debug.Assert(movement.PlayerId == actor.PlayerId);

            movement.Position = spawn.Position;
            movement.HorizontalRotation = spawn.Rotation;

            /* Let the other actors know it has spawned. */
            foreach (var otherActor in Actors)
            {
                otherActor.Peer.Events.Game.SendPlayerRespawned(
                    actor.Cmid,
                    spawn.Position,
                    spawn.Rotation
                );
            }

            Log.Debug($"{actor.GetDebug()} spawned at {spawn}.");
        }

        /* Determines if the vicitim can get damaged by the attcker. */
        protected abstract bool CanDamage(GameActor victim, GameActor attacker);

        /* Does damage and returns true if victim is killed; otherwise false. */
        protected bool DoDamage(GameActor victim, GameActor attacker, Weapon weapon, short damage, BodyPart part, out Vector3 direction)
        {
            bool selfDamage = victim.Cmid == attacker.Cmid;

            /* Calculate the direction of the hit. */
            var victimPos = victim.Movement.Position;
            var attackerPos = attacker.Movement.Position;
            direction = attackerPos - victimPos;

            if (!victim.Info.IsAlive)
                return false;

            /* Check if we can apply the damage on the players. */
            if (!CanDamage(victim, attacker))
                return false;

            float angle = Vector3.Angle(direction, new Vector3(0, 0, -1));
            if (direction.x < 0)
                angle = 360 - angle;

            byte byteAngle = Conversions.Angle2Byte(angle);

            /* Check if not self-damage. */
            if (!selfDamage)
                victim.Damages.Add(byteAngle, damage, part, 0, 0);
            else
                damage /= 2;

            /* Calculate armor absorption. */
            int armorDamage;
            int healthDamage;
            if (victim.Info.ArmorPoints > 0)
            {
                armorDamage = (byte)(victim.Info.GetAbsorptionRate() * damage);
                healthDamage = (short)(damage - armorDamage);
            }
            else
            {
                armorDamage = 0;
                healthDamage = damage;
            }

            int newArmor = victim.Info.ArmorPoints - armorDamage;
            int newHealth = victim.Info.Health - healthDamage;

            if (newArmor < 0)
                newHealth += newArmor;

            victim.Info.ArmorPoints = (byte)Math.Max(0, newArmor);
            victim.Info.Health = (short)Math.Max(0, newHealth);

            /* Record some statistics. */
            victim.Statistics.RecordDamageReceived(damage);
            attacker.Statistics.RecordHit(weapon.GetView().ItemClass);
            attacker.Statistics.RecordDamageDealt(weapon.GetView().ItemClass, damage);

            /* Check if the player is dead. */
            if (victim.Info.Health <= 0)
            {
                if (victim.Damages.Count > 0)
                {
                    /* 
                     * Force a push of damage events to the victim peer, so he
                     * gets the feedback of where he was hit from aka red hit
                     * marker HUD.
                     */
                    victim.Peer.Events.Game.SendDamageEvent(victim.Damages);
                    victim.Peer.Flush();

                    victim.Damages.Clear();
                }

                if (selfDamage)
                    attacker.Info.Kills--;
                else
                    attacker.Info.Kills++;

                victim.Info.Deaths++;

                /* Record statistics. */
                victim.Statistics.RecordDeath();

                if (selfDamage)
                {
                    attacker.Statistics.RecordSuicide();
                }
                else
                {
                    if (part == BodyPart.Head)
                        attacker.Statistics.RecordHeadshot();
                    else if (part == BodyPart.Nuts)
                        attacker.Statistics.RecordNutshot();

                    attacker.Statistics.RecordKill(weapon.GetView().ItemClass);
                }

                return true;
            }

            return false;
        }

        public string GetDebug()
        {
            return $"(room \"{GetView().Name}\":{RoomId} state {State.Current})";
        }

        public GameRoomDataView GetView()
        {
            _view.ConnectedPlayers = _players.Count;
            return _view;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                Loop.Dispose();

            _disposed = true;
        }
    }
}
