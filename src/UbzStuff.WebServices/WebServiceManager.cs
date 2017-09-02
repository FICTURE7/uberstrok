using log4net;
using System;
using System.ServiceModel;

namespace UbzStuff.WebServices
{
    public class WebServiceManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WebServiceManager));

        public WebServiceManager()
        {
            // Load main config at configs/main.json or
            // create a new default main config file if it does not exists.
            var config = Utils.DeserializeJsonAt<WebServiceConfiguration>("configs/main.json");
            if (config == null)
            {
                config = WebServiceConfiguration.Default;
                Utils.SerializeJsonAt("configs/main.json", config);
            }

            _config = config;

            _ctx = new WebServiceContext(this);

            _users = new UserManager(_ctx);
            _items = new ItemManager(_ctx);

            _services = new WebServiceCollection(_ctx);

            _binding = new BasicHttpBinding();
        }

        public UserManager Users => _users;
        public ItemManager Items => _items;

        public WebServiceConfiguration Configuration => _config;
        public WebServiceCollection Services => _services;

        // Figure out if the services is running.
        private bool _started;

        private readonly UserManager _users;
        private readonly ItemManager _items;

        private readonly WebServiceConfiguration _config;
        private readonly WebServiceCollection _services;
        private readonly WebServiceContext _ctx;

        private readonly BasicHttpBinding _binding;

        public void Start()
        {
            if (_started)
                throw new InvalidOperationException("Web services already started.");

            Log.Info("Initializing & starting web services...");
            Log.Info("Binding contracts...");

            try
            {
                // Bind the services to the HTTP endpoint.
                Services.Bind(_binding);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex);
                Log.Fatal("Unable to bind contracts to endpoint.");
                throw;
            }

            try
            {
                // Open services once we done binding them.
                Services.Open();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex);
                Log.Fatal("Unable to open service hosts.");
                throw;
            }

            _started = true;
        }
    }
}
