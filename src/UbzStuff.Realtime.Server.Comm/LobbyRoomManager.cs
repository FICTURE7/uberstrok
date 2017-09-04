using System.Collections.Generic;
using UbzStuff.Core.Views;

namespace UbzStuff.Realtime.Server.Comm
{
    public class LobbyRoomManager
    {
        public static LobbyRoomManager Instance => s_instance;
        private static LobbyRoomManager s_instance = new LobbyRoomManager();

        private LobbyRoomManager()
        {
            _actors = new List<CommActor>();
        }

        public List<CommActor> Actors => _actors;
        public List<CommActorInfoView> ActorViews
        {
            get
            {
                var list = new List<CommActorInfoView>(Actors.Count);
                for (int i = 0; i < Actors.Count; i++)
                    list.Add(Actors[i].View);

                return list;
            }
        }

        private readonly List<CommActor> _actors;

        public void UpdateList()
        {
            lock (_actors)
            {
                var views = ActorViews;
                for (int i = 0; i < Actors.Count; i++)
                {
                    var actor = Actors[i];
                    actor.Peer.Lobby.Events.SendFullPlayerListUpdate(views);
                }
            }
        }
    }
}
