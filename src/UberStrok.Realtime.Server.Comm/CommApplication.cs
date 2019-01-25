using log4net;
using log4net.Config;
using Photon.SocketServer;
using System.IO;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommApplication : ApplicationBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CommApplication));

        private LobbyRoomManager _lobbies;

        public static new CommApplication Instance => (CommApplication)ApplicationBase.Instance;
        public LobbyRoomManager Lobbies => _lobbies;

        protected override void Setup()
        {
            /* Add the log path to the properties so can use them in log4net.config. */
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationPath, "log");

            /* Configure log4net to use the log4net.config file. */
            var configFilePath = Path.Combine(BinaryPath, "log4net.config");
            var configFile = new FileInfo(configFilePath);
            if (configFile.Exists)
                XmlConfigurator.ConfigureAndWatch(configFile);

            _lobbies = new LobbyRoomManager();
            _log.Info("Started CommServer...");
        }

        protected override void TearDown()
        {
            _log.Info("Stopped CommServer...");
        }

        protected override PeerBase CreatePeer(InitRequest request)
        {
            _log.Info($"Accepted new connection at {request.RemoteIP}:{request.RemotePort}");

            /* Create CommPeer and CommActor instance. */
            var peer = new CommPeer(request);
            var actor = new CommActor(peer);

            /* Find available lobby and make the actor join that lobby. */
            var lobby = Lobbies.FindAvailable();
            actor.Join(lobby);

            return actor.Peer;
        }
    }
}
