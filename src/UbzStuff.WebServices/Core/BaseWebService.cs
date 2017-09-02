using System;

namespace UbzStuff.WebServices.Core
{
    public abstract class BaseWebService
    {
        protected BaseWebService(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;
        }

        protected WebServiceContext Context => _ctx;
        protected UserManager Users => _ctx.Users;

        private readonly WebServiceContext _ctx;

        protected internal virtual bool SetUp()
        {
            return true;
        }
    }
}
