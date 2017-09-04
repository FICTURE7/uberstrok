using log4net;
using System;
using System.Diagnostics;
using System.ServiceModel;

namespace UbzStuff.WebServices
{
    public class WebServiceManager
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(WebServiceManager).Name);

        public WebServiceManager()
        {
            Log.Info("Initializing web services...");

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

            try
            {
                _users = new UserManager(_ctx);
                _items = new ItemManager(_ctx);
                _servers = new ServerManager(_ctx);
                _maps = new MapManager(_ctx);
                _services = new WebServiceCollection(_ctx);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex);
                Log.Fatal("Unable to initialize web services.");
                throw;
            }

            _binding = new BasicHttpBinding();
        }

        public UserManager Users => _users;
        public ItemManager Items => _items;
        public ServerManager Servers => _servers;
        public MapManager Maps => _maps;

        public WebServiceConfiguration Configuration => _config;
        public WebServiceCollection Services => _services;

        // Figure out if the services is running.
        private bool _started;

        private readonly UserManager _users;
        private readonly ItemManager _items;
        private readonly ServerManager _servers;
        private readonly MapManager _maps;

        private readonly WebServiceConfiguration _config;
        private readonly WebServiceCollection _services;
        private readonly WebServiceContext _ctx;

        private readonly BasicHttpBinding _binding;

        public void Start()
        {
            if (_started)
                throw new InvalidOperationException("Web services already started.");

            Log.Info("Binding contracts...");

            var sw = Stopwatch.StartNew();

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

            Log.Info("Opening services...");

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

            sw.Stop();
            Log.Info($"Done in {sw.Elapsed.TotalSeconds}s.");
            _started = true;
        }
    }
}
