using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core
{
    public class PowerUpManager
    {
        private readonly List<Timer> _respawnTimers;
        private readonly Loop _loop;
        
        public bool IsLoaded { get; private set; }
        public List<int> Respawning { get; }

        public event Action<int, PickupItemType, byte> Picked;
        public event Action<int> Spawned;

        public PowerUpManager(Loop loop)
        {
            _loop = loop ?? throw new ArgumentNullException(nameof(loop));
            _respawnTimers = new List<Timer>();

            IsLoaded = false;
            Respawning = new List<int>();
        }

        public void Load(List<ushort> respawnTimes)
        {
            if (respawnTimes == null)
                throw new ArgumentNullException(nameof(respawnTimes));

            var length = respawnTimes.Count;

            _respawnTimers.Capacity = length;
            Respawning.Capacity = length;

            for (int i = 0; i < length; i++)
            {
                var interval = respawnTimes[i] * 1000;
                var timer = new Timer(_loop, interval);
                timer.Elapsed += OnRespawnTick;

                _respawnTimers.Add(timer);
            }

            IsLoaded = true;
        }

        public void PickUp(int pickupId, PickupItemType type, byte value)
        {
            if (pickupId < 0 || pickupId > _respawnTimers.Count - 1)
                return;

            /* Check if the pickup is currently respawning. */
            if (Respawning.Contains(pickupId))
                return;

            Respawning.Add(pickupId);
            _respawnTimers[pickupId].Restart();

            OnPicked(pickupId, type, value);

            /*
            foreach (var otherPeer in _room.Peers)
                otherPeer.Events.Game.SendPowerUpPicked(pickupId, 1);

            switch (type)
            {
                case PickupItemType.Health:
                    peer.Actor.Info.Health = (short)MathUtils.Clamp(peer.Actor.Info.Health + value, 0, 200);
                    break;
                case PickupItemType.Armor:
                    peer.Actor.Info.ArmorPoints = (byte)MathUtils.Clamp(peer.Actor.Info.ArmorPoints + value, 0, 200);
                    break;
            } 
            */
        }

        public void Tick()
        {
            foreach (var timer in _respawnTimers)
                timer.Tick();

            /*
            for (int i = 0; i < Respawning.Count; i++)
            {
                var time = _respawnTimes[Respawning[i]];
                var newTime = time.Subtract(_loop.DeltaTime);
                if (newTime.TotalMilliseconds <= 0)
                {
                    _respawnTimes[Respawning[i]] = TimeSpan.FromSeconds(0);
                    foreach (var otherPeer in _room.Peers)
                        otherPeer.Events.Game.SendPowerUpPicked(Respawning[i], 0);
                    OnSpawned(Respawning[i]);
                    
                    Respawning.RemoveAt(i);
                }
                else
                {
                    _respawnTimes[Respawning[i]] = newTime;
                }
            }
            */
        }

        private void OnRespawnTick()
        {
            /*
            Respawning.RemoveAt(pickupId);
            _respawnTimers[pickupId].Stop();

            OnSpawned(pickupId);
            */
        }

        protected virtual void OnPicked(int pickupId, PickupItemType type, byte value)
        {
            Picked?.Invoke(pickupId, type, value);
        }

        protected virtual void OnSpawned(int pickupId)
        {
            Spawned?.Invoke(pickupId);
        }
    }
}
