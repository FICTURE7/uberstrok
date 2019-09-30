using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.WebServices
{
    public class OldMapManager
    {
        public OldMapManager(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;

            var maps = Utils.DeserializeJsonAt<List<MapView>>("configs/game/maps.json");
            if (maps == null)
                throw new FileNotFoundException("configs/game/maps.json file not found.");

            _maps = maps;
        }

        private readonly List<MapView> _maps;
        private readonly WebServiceContext _ctx;

        public List<MapView> GetAll()
        {
            return _maps;
        }

        public MapView Get(string sceneName)
        {
            if (sceneName == null)
                throw new ArgumentNullException(nameof(sceneName));

            for (int i = 0; i < _maps.Count; i++)
            {
                var map = _maps[i];
                if (map.SceneName == sceneName)
                    return map;
            }

            return null;
        }
    }
}
