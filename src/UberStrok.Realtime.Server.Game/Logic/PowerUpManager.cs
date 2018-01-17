using System;
using System.Collections.Generic;

namespace UberStrok.Realtime.Server.Game.Logic
{
    public class PowerUpManager
    {
        private List<TimeSpan> _powerUpRespawnTimes;

        public PowerUpManager()
        {
            // Space
        }

        public bool IsLoaded() => _powerUpRespawnTimes != null;

        public void Load(List<ushort> respawnTimes)
        {
            var length = respawnTimes.Count;
            _powerUpRespawnTimes = new List<TimeSpan>(length);

            for (int i = 0; i < length; i++)
            {
                var time = TimeSpan.FromSeconds(respawnTimes[i]);
                _powerUpRespawnTimes.Add(time);
            }
        }
    }
}
