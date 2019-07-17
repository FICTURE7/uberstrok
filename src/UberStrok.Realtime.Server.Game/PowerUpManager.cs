using log4net;
using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    /* TODO: Move this to UberStrok.Core. */

    public class PowerUpManager
    {
        private readonly static ILog Log = LogManager.GetLogger(nameof(PowerUpManager));

        private List<TimeSpan> _respawnTimesOriginal;
        private List<TimeSpan> _respawnTimes;
        private List<int> _respawning;

        private readonly GameRoom _room;

        public PowerUpManager(GameRoom room)
        {
            _room = room ?? throw new ArgumentNullException(nameof(room));
        }

        public bool IsLoaded => _respawnTimesOriginal != null;
        public List<int> Respawning => _respawning;

        public void Reset()
        {
            if (!IsLoaded)
                return;

            for (int i = 0; i < _respawnTimesOriginal.Count; i++)
                _respawnTimes[i] = _respawnTimesOriginal[i];

            _respawning.Clear();
        }

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

        public void PickUp(GameActor actor, int pickupId, PickupItemType type, byte value)
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

            foreach (var otherActor in _room.Actors)
                otherActor.Peer.Events.Game.SendPowerUpPicked(pickupId, 1);

            switch (type)
            {
                case PickupItemType.Health:
                    actor.Info.Health = (short)MathUtils.Clamp(actor.Info.Health + value, 0, 200);
                    actor.Statistics.RecordHealthPickedUp();
                    break;
                case PickupItemType.Armor:
                    actor.Info.ArmorPoints = (byte)MathUtils.Clamp(actor.Info.ArmorPoints + value, 0, 200);
                    actor.Statistics.RecordArmorPickedUp();
                    break;
            } 
        }

        public void Tick()
        {
            if (!IsLoaded)
                return;

            for (int i = 0; i < _respawning.Count; i++)
            {
                var time = _respawnTimes[_respawning[i]];
                var newTime = time.Subtract(TimeSpan.FromMilliseconds(_room.Loop.DeltaTime));
                if (newTime.TotalMilliseconds <= 0)
                {
                    _respawnTimes[_respawning[i]] = TimeSpan.FromSeconds(0);
                    foreach (var otherActor in _room.Actors)
                        otherActor.Peer.Events.Game.SendPowerUpPicked(_respawning[i], 0);
                    
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
