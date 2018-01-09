using log4net;
using log4net.Config;
using Photon.SocketServer;
using System.IO;

namespace UberStrok.Realtime.Server.Game
{
    public class GameApplication : ApplicationBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GameApplication));

        public static new GameApplication Instance => (GameApplication)ApplicationBase.Instance;

        public GameManager Games => _games;
        public JobManager Jobs => _jobs;

        private GameManager _games;
        private JobManager _jobs;

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            Log.Info($"Accepted new connection at {initRequest.RemoteIP}:{initRequest.RemotePort}");

            return new GamePeer(initRequest);
        }

        protected override void Setup()
        {
            // Add a the log path to the properties so can use them in log4net.config.
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationPath, "log");
            // Configure log4net to use log4net.config file.
            var configFile = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if (configFile.Exists)
                XmlConfigurator.ConfigureAndWatch(configFile);

            Log.Info("Started GameServer...");

            _jobs = new JobManager();
            _games = new GameManager();

            var rooom = new TestRoom();
        }

        protected override void TearDown()
        {
            Log.Info("Stopped CommServer...");
        }

        private class TestRoom : BaseRooms<GamePeer>
        {
            public int Id { get; set; }
            public string Password { get; set; }

            public override void OnJoin(GamePeer peer)
            {
                
            }

            public override void OnLeave(GamePeer peer)
            {

            }
        }
    }
}
