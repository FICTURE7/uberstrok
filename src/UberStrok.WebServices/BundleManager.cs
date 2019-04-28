using System;
using System.IO;
using UberStrok.Core.Views;
/* using System.Collections.Generic;

namespace UberStrok.WebServices
{
    public class BundleManager
    {
        public BundleManager(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;

            var bundles = Utils.DeserializeJsonAt<BundleView>("configs/game/bundles.json");
            if (bundles == null)
                throw new FileNotFoundException("configs/game/bundles.json file not found.");

            _bundles = bundles;
        }

        private readonly BundleView _bundles;
        private readonly WebServiceContext _ctx;


        public BundleView GetBundles()
        {
            return _bundles;
        }
    }
}

*/

