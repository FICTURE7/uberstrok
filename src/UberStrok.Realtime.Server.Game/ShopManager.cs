using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server.Game
{
    public class ShopManager
    {
        private static readonly ILog s_log = LogManager.GetLogger(nameof(ShopManager));

        private bool _isLoaded;

        private Dictionary<int, UberStrikeItemFunctionalView> _functionalItems;
        private Dictionary<int, UberStrikeItemGearView> _gearItems;
        private Dictionary<int, UberStrikeItemQuickView> _quickItems;
        private Dictionary<int, UberStrikeItemWeaponView> _weaponItems;

        public bool IsLoaded => _isLoaded;
        public Dictionary<int, UberStrikeItemFunctionalView> FunctionalItems => _functionalItems;
        public Dictionary<int, UberStrikeItemGearView> GearItems => _gearItems;
        public Dictionary<int, UberStrikeItemQuickView> QuickItems => _quickItems;
        public Dictionary<int, UberStrikeItemWeaponView> WeaponItems => _weaponItems;

        public void Load(string authToken)
        {
            if (authToken == null)
                throw new ArgumentNullException(nameof(authToken));

            //TODO: Provide some base class for this kind of server-server communications.
            var bytes = Convert.FromBase64String(authToken);
            var data = Encoding.UTF8.GetString(bytes);

            var webServer = data.Substring(0, data.IndexOf("#####"));

            s_log.Debug($"Retrieving loadout data {authToken} from the web server {webServer}");

            // Retrieve loadout data from the web server.
            var client = new ShopWebServiceClient(webServer);
            var shopView = client.GetShop();

            _functionalItems = LoadDictionary(shopView.FunctionalItems);
            _gearItems = LoadDictionary(shopView.GearItems);
            _quickItems = LoadDictionary(shopView.QuickItems);
            _weaponItems = LoadDictionary(shopView.WeaponItems);

            _isLoaded = true;
        }

        private Dictionary<int, TUberStrikeItem> LoadDictionary<TUberStrikeItem>(List<TUberStrikeItem> list) where TUberStrikeItem : BaseUberStrikeItemView
        {
            var dict = new Dictionary<int, TUberStrikeItem>(list.Count);
            foreach (var item in list)
                dict.Add(item.ID, item);

            return dict;
        }
    }
}
