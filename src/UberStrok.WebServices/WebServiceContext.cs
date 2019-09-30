using System;

namespace UberStrok.WebServices
{
    public class WebServiceContext
    {
        internal WebServiceContext(WebServiceManager ws)
        {
            if (ws == null)
                throw new ArgumentNullException(nameof(ws));

            _ws = ws;
        }

        public string ServiceBase => _ws.Configuration.ServiceBase;

        public UserManager Users => _ws.Users;
        public ItemManager Items => _ws.Items;
        public ServerManager Servers => _ws.Servers;
        public OldMapManager Maps => _ws.Maps;

        public WebServiceConfiguration Configuration => _ws.Configuration;

        private readonly WebServiceManager _ws;
    }
}
