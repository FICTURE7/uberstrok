using log4net;
using log4net.Config;
using Newtonsoft.Json;
using Photon.SocketServer;
using System;
using System.IO;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommApplication : ApplicationBase
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(CommApplication));

        public static new CommApplication Instance => (CommApplication)ApplicationBase.Instance;
        public LobbyRoomManager Rooms { get; private set; }
        public CommConfiguration Configuration { get; private set; }

        private void LoadLog4NetConfig()
        {
            /* Add a the log path to the properties so can use them in log4net.config. */
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationPath, "log");
            /* Configure log4net to use log4net.config file. */
            var configFile = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if (configFile.Exists)
                XmlConfigurator.ConfigureAndWatch(configFile);
        }

        private void LoadCommConfig()
        {
            var path = Path.Combine(BinaryPath, "comm.json");

            Log.Info($"Loading configuration at {path}");
            if (!File.Exists(path))
            {
                Configuration = CommConfiguration.Default;
                Log.Info("comm.json not found, using default configuration.");
            }
            else
            {
                var json = File.ReadAllText(path);
                Configuration = JsonConvert.DeserializeObject<CommConfiguration>(json);

                if (Configuration.CompositeHash != null && Configuration.CompositeHash.Length != 64)
                    throw new Exception("Composite hash was incorrectly configured.");
                if (Configuration.JunkHash != null && Configuration.JunkHash.Length != 64)
                    throw new Exception("Junk hash was incorrectly configured.");

                Log.Info($"CompositeHash: {Configuration.CompositeHash} JunkHash: {Configuration.JunkHash}.");
            }
        }

        protected override void Setup()
        {
            /* Load configurations. */
            LoadLog4NetConfig();
            LoadCommConfig();

            Rooms = new LobbyRoomManager();
            Log.Info("Started CommServer...");
        }

        protected override void TearDown()
        {
            Log.Info("Stopped CommServer...");
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            Log.Info($"Accepted new connection at {initRequest.RemoteIP}:{initRequest.RemotePort}.");
            return new CommPeer(initRequest);
        }
    }
}
