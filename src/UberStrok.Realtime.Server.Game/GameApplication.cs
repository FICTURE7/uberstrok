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
        private UberStrok.Game _main;
        /* Game room manager. */
        private GameRoomManager _rooms;

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

            /* Configure log4net to use log4net.config file. */
            var configFile = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if (configFile.Exists)
                XmlConfigurator.ConfigureAndWatch(configFile);

            _main = new UberStrok.Game();
            _rooms = new GameRoomManager();

            Log.Info("Started GameServer...");
        }

        protected override void TearDown()
        {
            Log.Info("Stopped CommServer...");
        }
    }
}
