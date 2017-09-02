using System;

namespace UbzStuff.WebServices
{
    public class ServerManager
    {
        public ServerManager(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;
        }

        private readonly WebServiceContext _ctx;
    }
}
