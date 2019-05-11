using log4net;
using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public class PowerUpManager
    {
        private readonly static ILog Log = LogManager.GetLogger(nameof(PowerUpManager));

        private List<TimeSpan> _respawnTimesOriginal;
        private List<TimeSpan> _respawnTimes;
        private List<int> _respawning;

        private readonly BaseGameRoom _room;

        public PowerUpManager(BaseGameRoom room)
        {
            _room = room ?? throw new ArgumentNullException(nameof(room));
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
                Log.Warn($"Unknown power-up with ID: {pickupId}");
                return;
            }

            if (_respawning.Contains(pickupId))
                return;

            _respawnTimes[pickupId] = _respawnTimesOriginal[pickupId];
            _respawning.Add(pickupId);

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
                    
                    Log.Debug($"Respawned power-up with ID: {_respawning[i]}");
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
