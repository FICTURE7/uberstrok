using System;

namespace UbzStuff.WebServices
{
    public class ItemManager
    {
        public ItemManager(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;
        }

        private readonly WebServiceContext _ctx;


    }
}
