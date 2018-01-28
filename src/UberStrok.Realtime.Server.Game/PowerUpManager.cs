using log4net;
using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public class PowerUpManager
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(PowerUpManager));

        private List<TimeSpan> _respawnTimesOriginal;
        private List<TimeSpan> _respawnTimes;
        private List<int> _respawning;

        private readonly BaseGameRoom _room;

        public PowerUpManager(BaseGameRoom room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            _room = room;
        }

        public bool IsLoaded => _respawnTimesOriginal != null;
        public List<int> Respawning => _respawning;

        public void Load(List<ushort> respawnTimes)
        {
            var length = respawnTimes.Count;
            _respawnTimesOriginal = new List<TimeSpan>(length);
            _respawnTimes = new List<TimeSpan>(length);
            _respawning = new List<int>(length);

            for (int i = 0; i < length; i++)
            {
                var time = TimeSpan.FromSeconds(respawnTimes[i]);

                _respawnTimesOriginal.Add(time);
                _respawnTimes.Add(TimeSpan.FromSeconds(0));
            }
        }

        public void PickUp(GamePeer peer, int pickupId, PickupItemType type, byte value)
        {
            if (pickupId < 0 || pickupId > _respawnTimesOriginal.Count - 1)
            {
                s_log.Warn($"Unknown power-up with ID: {pickupId}");
                return;
            }

            /* TODO: Check if the thing is respawning before doing anything. */
            _respawnTimes[pickupId] = _respawnTimesOriginal[pickupId];
            _respawning.Add(pickupId);

            foreach (var otherPeer in _room.Peers)
                otherPeer.Events.Game.SendPowerUpPicked(pickupId, 1);

            switch (type)
            {
                case PickupItemType.Health:
                    peer.Actor.Info.Health += value;
                    break;
                case PickupItemType.Armor:
                    peer.Actor.Info.ArmorPoints += value;
                    break;
            } 
        }

        public void Update()
        {
            for (int i = 0; i < _respawning.Count; i++)
            {
                var time = _respawnTimes[_respawning[i]];
                var newTime = time.Subtract(_room.Loop.DeltaTime);
                if (newTime.TotalMilliseconds <= 0)
                {
                    _respawnTimes[_respawning[i]] = TimeSpan.FromSeconds(0);
                    foreach (var otherPeer in _room.Peers)
                        otherPeer.Events.Game.SendPowerUpPicked(_respawning[i], 0);
                    
                    s_log.Debug($"Respawned power-up with ID: {_respawning[i]}");
                    _respawning.RemoveAt(i);
                }
                else
                {
                    _respawnTimes[_respawning[i]] = newTime;
                }
            }
        }
    }
}
