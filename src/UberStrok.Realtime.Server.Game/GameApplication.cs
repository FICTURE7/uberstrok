using log4net;
using log4net.Config;
using Photon.SocketServer;
using System.IO;
using UberStrok.Realtime.Server.Game.Events;

namespace UberStrok.Realtime.Server.Game
{
    public class GameApplication : ApplicationBase
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(GameApplication));

        public static new GameApplication Instance => (GameApplication)ApplicationBase.Instance;

        /* Main/Lobby room. */
        private GameLobbyRoom _lobby;
        /* Game room manager. */
        private GameRoomManager _rooms;

        public GameLobbyRoom Lobby => _lobby;
        public GameRoomManager Rooms => _rooms;

        protected override PeerBase CreatePeer(InitRequest request)
        {
            Log.Info($"Accepted new connection at {request.RemoteIP}:{request.RemotePort}");

            var peer = new GamePeer(request);
            var actor = new GameActor();

            /* Add the actor the players in the main lobby/room */
            _lobby.Join(actor);
            _lobby.OnEvent(new PeerJoinedEvent
            {
                Peer = peer,
                Actor = actor
            });

            return peer;
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

            _lobby = new GameLobbyRoom();

            Log.Info("Started GameServer...");
        }

        protected override void TearDown()
        {
            Log.Info("Stopped CommServer...");
        }
    }
}
