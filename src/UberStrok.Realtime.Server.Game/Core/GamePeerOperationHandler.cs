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
                rooms.Add(room.View);

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

            peer.AuthToken = authToken;
            peer.Member = GetMemberFromAuthToken(authToken);
            peer.Loadout = GetLoadoutFromAuthToken(authToken);

            var room = default(BaseGameRoom);
            try
            {
                room = GameApplication.Instance.Rooms.Create(roomData, password);
                room.ShopManager.Load(authToken);

                /* Enable QUICK-SWATCH boiiiii */
                room.View.GameFlags |= 4;
            }
            catch (NotSupportedException)
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "UberStrok does not support the selected game mode.");
                s_log.Debug($"OnCreateRoom: Client tried to create a game mode which is not supported.");
                return;
            }
            catch (Exception ex)
            {
#if !DEBUG
                var message = "Failed to create game room.";
#else
                var message = "Failed to create game room. \nCheck logs: " + ex.Message;
#endif

                peer.Events.SendRoomEnterFailed(string.Empty, 0, message);
                s_log.Error($"OnCreateRoom: Unable to create game room.", ex);
                return;
            }

            room.Join(peer);
            s_log.Debug($"OnCreateRoom: Created new room: {room.Number} and made the client to join it.");
        }

        protected override void OnJoinRoom(GamePeer peer, int roomId, string password, string clientVersion, string authToken, string magicHash)
        {
            /* Check the client version. */
            if (clientVersion != "4.7.1")
                peer.Disconnect();

            peer.AuthToken = authToken;
            peer.Member = GetMemberFromAuthToken(authToken);
            peer.Loadout = GetLoadoutFromAuthToken(authToken);

            var room = GameApplication.Instance.Rooms.Get(roomId);
            if (room != null)
            {
                s_log.Debug($"OnJoinRoom: Room: {roomId} IsPasswordProcted: {room.View.IsPasswordProtected}");

                /* Request password if the room is password protected & check password.*/
                if (room.View.IsPasswordProtected && password != room.Password)
                    peer.Events.SendRequestPasswordForRoom(room.View.Server.ConnectionString, room.Number);
                else
                    room.Join(peer);
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
            var room = peer.Room;

            if (peer.Room != null)
            {
                peer.Room.Leave(peer);
                if (room.Peers.Count <= 0)
                    GameApplication.Instance.Rooms.Remove(room.Number);
            }
            else
                /* wtf fam?*/
                s_log.Error("A client tried to a leave a game room even though it was not in a room.");

            
        }

        protected override void OnUpdatePing(GamePeer peer, ushort ping)
        {
            /* Ignore ping value sent by client, cause we use the server side only. */
            peer.Actor.Info.Ping = (ushort)(peer.RoundTripTime / 2);
        }

        protected override void OnUpdateLoadout(GamePeer peer)
        {
            // Update the loadout while ingame.
            // There is an approximate 5 second delay after this request for the loadout to take effect on other clients' sides.
            // Maybe add a join delay after changing loadout?
            var loadout = GetLoadoutFromAuthToken(peer.AuthToken);
            var weapons = new List<int> {
                loadout.MeleeWeapon,
                loadout.Weapon1,
                loadout.Weapon2,
                loadout.Weapon3
            };
            peer.Actor.Info.Weapons = weapons;
            var gear = new List<int>
            {
                (int)loadout.Type,
                loadout.Head,
                loadout.Face,
                loadout.Gloves,
                loadout.UpperBody,
                loadout.LowerBody,
                loadout.Boots
            };
            peer.Actor.Info.Gear = gear;
        }

        protected override void OnUpdateKeyState(GamePeer peer, byte state)
        {
            // Space
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

        private LoadoutView GetLoadoutFromAuthToken(string authToken)
        {
            //TODO: Provide some base class for this kind of server-server communications.

            var bytes = Convert.FromBase64String(authToken);
            var data = Encoding.UTF8.GetString(bytes);

            var webServer = data.Substring(0, data.IndexOf("#####"));

            s_log.Debug($"Retrieving loadout data {authToken} from the web server {webServer}");

            // Retrieve loadout data from the web server.
            var client = new UserWebServiceClient(webServer);
            var loadout = client.GetLoadout(authToken);
            return loadout;
        }
    }
}