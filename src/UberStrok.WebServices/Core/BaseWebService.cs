using System;

namespace UberStrok.WebServices.Core
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

        private readonly WebServiceContext _ctx;
    }
}
