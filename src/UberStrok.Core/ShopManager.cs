using System;
using System.Collections.Generic;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Core
{
    public class ShopManager
    {
        public bool IsLoaded { get; private set; }
        public Dictionary<int, UberStrikeItemFunctionalView> FunctionalItems { get; private set; }
        public Dictionary<int, UberStrikeItemGearView> GearItems { get; private set; }
        public Dictionary<int, UberStrikeItemQuickView> QuickItems { get; private set; }
        public Dictionary<int, UberStrikeItemWeaponView> WeaponItems { get; private set; }

        public void Load(string webServices, string authToken)
        {
            if (webServices == null)
                throw new ArgumentNullException(nameof(webServices));
            if (authToken == null)
                throw new ArgumentNullException(nameof(authToken));

            /* Retrieve loadout data from the web server. */
            var client = new ShopWebServiceClient(webServices);
            var shopView = client.GetShop();

            FunctionalItems = LoadDictionary(shopView.FunctionalItems);
            GearItems = LoadDictionary(shopView.GearItems);
            QuickItems = LoadDictionary(shopView.QuickItems);
            WeaponItems = LoadDictionary(shopView.WeaponItems);

            IsLoaded = true;
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
