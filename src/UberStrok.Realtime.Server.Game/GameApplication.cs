using log4net;
using log4net.Config;
using Photon.SocketServer;
using System.IO;

namespace UberStrok.Realtime.Server.Game
{
    public class GameApplication : ApplicationBase
    {
        private static readonly ILog s_log = LogManager.GetLogger(nameof(GameApplication));

        public static new GameApplication Instance => (GameApplication)ApplicationBase.Instance;

        private GameRoomManager _rooms;
        public GameRoomManager Rooms => _rooms;

        public int PlayerCount
        {
            get
            {
                /* Total players in all game rooms. */
                var sum = 0;
                foreach(var room in Rooms)
                    sum += room.Players.Count;

                return sum;
            }
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            s_log.Info($"Accepted new connection at {initRequest.RemoteIP}:{initRequest.RemotePort}");

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

            s_log.Info("Started GameServer...");

            _rooms = new GameRoomManager();
        }

        protected override void TearDown()
        {
            s_log.Info("Stopped CommServer...");
        }
    }
}
