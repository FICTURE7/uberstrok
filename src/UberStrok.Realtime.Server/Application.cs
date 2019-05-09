using log4net;
using log4net.Config;
using Newtonsoft.Json;
using Photon.SocketServer;
using System;
using System.IO;

namespace UberStrok.Realtime.Server
{
    public abstract class Application : ApplicationBase
    {
        public static new Application Instance => (Application)ApplicationBase.Instance;

        protected ILog Log { get; }
        public ApplicationConfiguration Configuration { get; private set; }
        private PeerConfiguration PeerConfiguration { get; set; }

        protected Application()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Log = LogManager.GetLogger(GetType().Name);
        }

        protected abstract void OnSetup();
        protected abstract void OnTearDown();
        protected abstract Peer OnCreatePeer(InitRequest initRequest);

        private void SetupLog4net()
        {
            /* Add a the log path to the properties so we can use them in log4net.config. */
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationPath, "log");
            /* Configure log4net to use the log4net.config file. */
            var configFile = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if (configFile.Exists)
                XmlConfigurator.ConfigureAndWatch(configFile);
        }

        private void SetupConfigs()
        {
            var path = Path.Combine(BinaryPath, "uberstrok.realtime.server.json");

            Log.Info($"Loading configuration at {path}");
            if (!File.Exists(path))
            {
                Configuration = ApplicationConfiguration.Default;
                Log.Info("uberstrok.realtime.server.json not found, using default configuration.");
            }
            else
            {
                try
                {
                    var json = File.ReadAllText(path);
                    Configuration = JsonConvert.DeserializeObject<ApplicationConfiguration>(json);
                    Configuration.Check();

                    Log.Info("uberstrok.realtime.server.json loaded ->");
                    Log.Info($"\tCompositeHashes({Configuration.CompositeHashBytes.Count})");
                    Log.Info($"\tJunkHashes({Configuration.JunkHashBytes.Count})");
                    Log.Info($"\tHeartbeatTimeout = {Configuration.HeartbeatTimeout}");
                    Log.Info($"\tHeartbeatInterval = {Configuration.HeartbeatInterval}");
                }
                catch (Exception ex)
                {
                    Log.Fatal("Failed to load or parse uberstrok.realtime.server.json", ex);
                    throw;
                }
            }

            PeerConfiguration = new PeerConfiguration
            (
                Configuration.WebServices,
                Configuration.WebServicesAuth,
                Configuration.HeartbeatTimeout,
                Configuration.HeartbeatInterval,
                Configuration.CompositeHashBytes,
                Configuration.JunkHashBytes
            );
        }

        protected sealed override void Setup()
        {
            SetupLog4net();
            SetupConfigs();

            OnSetup();

            Log.Info($"Setup {GetType().Name}... Complete");
        }

        protected sealed override void TearDown()
        {
            OnTearDown();

            Log.Info($"TearDown {GetType().Name}... Complete");
        }

        protected sealed override PeerBase CreatePeer(InitRequest initRequest)
        {
            Log.Info($"Accepted new connection at {initRequest.RemoteIP}:{initRequest.RemotePort}.");

            initRequest.UserData = PeerConfiguration;
            return OnCreatePeer(initRequest);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (e.IsTerminating)
                Log.Fatal("Unhandled exception", exception);
            else
                Log.Error("Unhandled exception", exception);
        }
    }
}
