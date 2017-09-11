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
            _sync = new object();
            _actors = new Dictionary<int, CommActor>();
        }

        public IEnumerable<CommActor> Actors => _actors.Values;

        private readonly object _sync;
        private readonly Dictionary<int, CommActor> _actors;

        public void UpdateList()
        {
            lock (_sync)
            {
                var views = new List<CommActorInfoView>(_actors.Count);
                foreach (var actor in Actors)
                    views.Add(actor.View);

                foreach (var actor in Actors)
                    actor.Peer.Lobby.Events.SendFullPlayerListUpdate(views);
            }
        }

        public void ChatToAll(CommActor actor, string message)
        {
            lock (_sync)
            {
                foreach (var a in Actors)
                {
                    // Don't send back the message sent by the actor to itself.
                    if (a.Cmid != actor.Cmid)
                        a.Peer.Lobby.Events.SendLobbyChatMessage(actor.Cmid, actor.Name, message);
                }
            }
        }

        public void Add(CommActor actor)
        {
            lock (_sync)
            {
                _actors.Add(actor.Cmid, actor);

                Log.Info($"{actor.Cmid} Joined the lobby");

                // Notify the peer that it entered the lobby.
                actor.Peer.Events.SendLobbyEntered();
            }
        }

        public void Remove(int cmid)
        {
            lock (_sync)
            {
                _actors.Remove(cmid);

                Log.Info($"{cmid} Left the lobby");
                //TODO: Tell the web servers to close the user's session or something.
            }
        }
    }
}
