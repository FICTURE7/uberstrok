using log4net;
using log4net.Config;
using Photon.SocketServer;
using System.IO;

namespace UberStrok.Realtime.Server.Game
{
    public class GameApplication : ApplicationBase
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(GameApplication));

        public static new GameApplication Instance => (GameApplication)ApplicationBase.Instance;

        /* Main game instance. */
        private GameWorld _main;
        /* Game room manager. */
        private GameRoomManager _rooms;

        public GameWorld Main => _main;
        public GameRoomManager Rooms => _rooms;

        public int PlayerCount
        {
            get
            {
                /* Total players in all game rooms. */
                var sum = 0;
                foreach (var room in Rooms)
                    sum += room.Players.Count;

                return sum;
            }
        }

        protected override PeerBase CreatePeer(InitRequest request)
        {
            Log.Info($"Accepted new connection at {request.RemoteIP}:{request.RemotePort}");
            return new GamePeer(request);
        }

        protected override void Setup()
        {
            /* Add the log path to the properties so can use them in log4net.config. */
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationPath, "log");

            /* Configure log4net to use the log4net.config file. */
            var configFilePath = Path.Combine(BinaryPath, "log4net.config");
            var configFile = new FileInfo(configFilePath);
            if (configFile.Exists)
                XmlConfigurator.ConfigureAndWatch(configFile);

            _main = new GameWorld();
            _rooms = new GameRoomManager();

            Log.Info("Started GameServer...");
        }

        protected override void TearDown()
        {
            Log.Info("Stopped CommServer...");
        }
    }
}
