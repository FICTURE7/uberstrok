using System;
using System.Collections.Generic;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class MapManager
    {
        private readonly List<MapView> _maps;

        public MapManager(IEnumerable<MapView> maps)
        {
            if (maps == null)
                throw new ArgumentNullException(nameof(maps));

            _maps = new List<MapView>();
            _maps.AddRange(maps);
        }

        public List<MapView> GetView()
            => _maps;
    }
}
