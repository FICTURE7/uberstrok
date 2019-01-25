using log4net;
using log4net.Config;
using Photon.SocketServer;
using System.IO;

namespace UberStrok.Realtime.Server.Game
{
    public class GameApplication : ApplicationBase
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(GameApplication));

        private GameRoomManager _games;

        public static new GameApplication Instance => (GameApplication)ApplicationBase.Instance;
        public GameRoomManager Games => _games;

        protected override void Setup()
        {
            /* Add the log path to the properties so can use them in log4net.config. */
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationPath, "log");

            /* Configure log4net to use the log4net.config file. */
            var configFilePath = Path.Combine(BinaryPath, "log4net.config");
            var configFile = new FileInfo(configFilePath);
            if (configFile.Exists)
                XmlConfigurator.ConfigureAndWatch(configFile);

            _games = new GameRoomManager();
            Log.Info("Started GameServer...");
        }

        protected override void TearDown()
        {
            Log.Info("Stopped GameServer...");
        }

        protected override PeerBase CreatePeer(InitRequest request)
        {
            Log.Info($"Accepted new connection at {request.RemoteIP}:{request.RemotePort}");
            return null;
        }
    }
}
