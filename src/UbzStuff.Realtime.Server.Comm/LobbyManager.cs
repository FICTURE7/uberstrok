using System.Collections.Generic;
using UbzStuff.Core.Views;

namespace UbzStuff.Realtime.Server.Comm
{
    public class LobbyManager
    {
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
                for (int i = 0; i < _actors.Count; i++)
                    views.Add(_actors[i].View);

                for (int i = 0; i < _actors.Count; i++)
                {
                    var actor = _actors[i];
                    actor.Peer.Lobby.Events.SendFullPlayerListUpdate(views);
                }
            }
        }

        public void Add(CommActor actor)
        {
            lock (_actors)
            {
                _actors.Add(actor.Cmid, actor);

                // Notify the peer that it entered the lobby.
                actor.Peer.Events.SendLobbyEntered();
            }
        }

        public void Remove(int cmid)
        {
            lock (_actors)
            {
                _actors.Remove(cmid);

                //TODO: Tell the web servers to close the user's session or something.
            }
        }
    }
}
