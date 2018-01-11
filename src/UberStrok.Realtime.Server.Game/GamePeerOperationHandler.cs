using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeerOperationHandler : BaseGamePeerOperationHandler
    {
        /* Be GC friendly a little bit. By allocating it once. */
        private readonly static PhotonServerLoadView s_loadView;
        private readonly static ILog s_log;

        static GamePeerOperationHandler()
        {
            s_log = LogManager.GetLogger(nameof(GamePeerOperationHandler));
            s_loadView = new PhotonServerLoadView
            {
                /* TODO: Implement some configs or somethings. */
                MaxPlayerCount = 100,
                State = PhotonServerLoadView.Status.Alive,
            };
        }

        protected override void OnGetGameListUpdates(GamePeer peer)
        {
            // TODO: Don't use that, cause apparently there is a FullGameList event which we can send to wipe stuff and things.
            // Clear the client list of games available.
            peer.Events.SendGameListUpdateEnd();

            var rooms = new List<GameRoomDataView>(GameApplication.Instance.Rooms.Count);
            foreach (var room in GameApplication.Instance.Rooms)
                rooms.Add(room.Data);

            peer.Events.SendGameListUpdate(rooms, new List<int>());

            s_log.Debug($"OnGetGameListUpdates: Room Count -> {rooms.Count}");
        }

        protected override void OnGetServerLoad(GamePeer peer)
        {
            /* UberStrike does not care about this value, it uses its client side value. */
            s_loadView.Latency = peer.RoundTripTime / 2;
            s_loadView.PeersConnected = GameApplication.Instance.PlayerCount;
            /* UberStrike also does not care about this value, it uses PeersConnected. */
            s_loadView.PlayersConnected = GameApplication.Instance.PlayerCount;
            s_loadView.RoomsCreated = GameApplication.Instance.Rooms.Count;
            s_loadView.TimeStamp = DateTime.UtcNow;

            peer.Events.SendServerLoadData(s_loadView);
            s_log.Debug($"OnGetServerLoad: Load -> {s_loadView.PeersConnected}/{s_loadView.MaxPlayerCount} Rooms: {s_loadView.RoomsCreated}");
        }

        protected override void OnCreateRoom(GamePeer peer, GameRoomDataView roomData, string password, string clientVersion, string authToken, string magicHash)
        {
            /* Check the client version. */
            if (clientVersion != "4.7.1")
                peer.Disconnect();

            peer.Member = GetMemberFromAuthToken(authToken);

            var room = GameApplication.Instance.Rooms.Create(roomData, password);
            if (room != null)
            {
                room.OnJoin(peer);
                s_log.Debug($"OnCreateRoom: Created new room: {room.Id} and made the client to join it.");
            }
            else
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "Room does not exist anymore.");
                s_log.Warn($"OnCreateRoom: Client wanted to create a room, but Rooms.Create returned null.");
            }
        }

        protected override void OnJoinRoom(GamePeer peer, int roomId, string password, string clientVersion, string authToken, string magicHash)
        {
            /* Check the client version. */
            if (clientVersion != "4.7.1")
                peer.Disconnect();

            peer.Member = GetMemberFromAuthToken(authToken);

            var room = GameApplication.Instance.Rooms.Get(roomId);
            if (room != null)
            {
                s_log.Debug($"OnJoinRoom: Room: {roomId} IsPasswordProcted: {room.Data.IsPasswordProtected}");

                /* Request password if the room is password protected & check password.*/
                if (room.Data.IsPasswordProtected && password != room.Password)
                    peer.Events.SendRequestPasswordForRoom(room.Data.Server.ConnectionString, room.Id);
                else
                    room.OnJoin(peer);
            }
            else
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "Room does not exist anymore.");
                s_log.Warn($"OnJoinRoom: Client wanted to join a room, but Rooms.Get returned null.");
            }
        }

        protected override void OnLeaveRoom(GamePeer peer)
        {
            //TODO: Kill room if the number of connected players is 0.
            if (peer.Room != null)
            {
                peer.Room.OnLeave(peer);
            }
            else
            {
                /* wtf fam?*/
                s_log.Error("A client tried to a leave a game room even though it was not in a room.");
            }
        }

        protected override void OnUpdatePing(GamePeer peer, ushort ping)
        {
            //TODO: Client should not have the ability to change its ping but it can cause uberstrike.
            peer.Ping = ping;
        }

        private UberstrikeUserView GetMemberFromAuthToken(string authToken)
        {
            //TODO: Provide some base class for this kind of server-server communications.

            var bytes = Convert.FromBase64String(authToken);
            var data = Encoding.UTF8.GetString(bytes);

            var webServer = data.Substring(0, data.IndexOf("#####"));

            s_log.Debug($"Retrieving user data {authToken} from the web server {webServer}");

            // Retrieve user data from the web server.
            var client = new UserWebServiceClient(webServer);
            var member = client.GetMember(authToken);
            return member;
        }
    }
}