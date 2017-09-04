using log4net;
using System.Collections.Generic;
using UbzStuff.Core.Views;

namespace UbzStuff.Realtime.Server.Comm
{
    public class LobbyManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LobbyManager));

        public static LobbyManager Instance => s_instance;
        private static LobbyManager s_instance = new LobbyManager();

        private LobbyManager()
        {
            _actors = new Dictionary<int, CommActor>();
        }

        public Dictionary<int, CommActor> Actors => _actors;

        private readonly Dictionary<int, CommActor> _actors;

        public void UpdateList()
        {
            lock (_actors)
            {
                var views = new List<CommActorInfoView>(_actors.Count);
                foreach (var actor in _actors.Values)
                    views.Add(actor.View);

                foreach (var actor in _actors.Values)
                    actor.Peer.Lobby.Events.SendFullPlayerListUpdate(views);
            }
        }

        public void Add(CommActor actor)
        {
            lock (_actors)
            {
                _actors.Add(actor.Cmid, actor);

                Log.Info($"{actor.Cmid} Joined the lobby");

                // Notify the peer that it entered the lobby.
                actor.Peer.Events.SendLobbyEntered();
            }
        }

        public void Remove(int cmid)
        {
            lock (_actors)
            {
                _actors.Remove(cmid);

                Log.Info($"{cmid} Left the lobby");
                //TODO: Tell the web servers to close the user's session or something.
            }
        }
    }
}
