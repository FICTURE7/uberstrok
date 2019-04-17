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
        private readonly static ILog Log = LogManager.GetLogger(nameof(GamePeerOperationHandler));

        /* Be GC friendly a little bit. By allocating it once. */
        private readonly static PhotonServerLoadView _loadView = new PhotonServerLoadView
        {
            /* TODO: Implement some configs or somethings. */
            MaxPlayerCount = 100,
            State = PhotonServerLoadView.Status.Alive,
        };

        protected override void OnGetGameListUpdates(GamePeer peer)
        {
            // TODO: Don't use that, cause apparently there is a FullGameList event which we can send to wipe stuff and things.
            // Clear the client list of games available.
            peer.Events.SendGameListUpdateEnd();

            var rooms = new List<GameRoomDataView>(GameApplication.Instance.Rooms.Count);
            foreach (var room in GameApplication.Instance.Rooms)
                rooms.Add(room.View);

            peer.Events.SendGameListUpdate(rooms, new List<int>());
            Log.Debug($"OnGetGameListUpdates: Room Count = {rooms.Count}");
        }

        protected override void OnGetServerLoad(GamePeer peer)
        {
            /* UberStrike does not care about this value, it uses its client side value. */
            _loadView.Latency = peer.RoundTripTime / 2;
            _loadView.PeersConnected = GameApplication.Instance.PlayerCount;
            /* UberStrike also does not care about this value, it uses PeersConnected. */
            _loadView.PlayersConnected = GameApplication.Instance.PlayerCount;
            _loadView.RoomsCreated = GameApplication.Instance.Rooms.Count;
            _loadView.TimeStamp = DateTime.UtcNow;

            peer.Events.SendServerLoadData(_loadView);
            Log.Debug($"OnGetServerLoad: Load -> {_loadView.PeersConnected}/{_loadView.MaxPlayerCount} Rooms: {_loadView.RoomsCreated}");
        }

        protected override void OnCreateRoom(GamePeer peer, GameRoomDataView roomData, string password, string clientVersion, string authToken, string magicHash)
        {
            /* Check the client version. */
            if (clientVersion != "4.7.1")
                peer.Disconnect();

            try
            {
                if (!peer.Authenticate(authToken, magicHash))
                {
                    peer.SendError();
                    return;
                }
            }
            catch
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "Failed to authenticate user. Try restarting UberStrike.");
                throw;
            }

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
                Log.Debug($"OnCreateRoom: Client tried to create a game mode which is not supported.");
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
                Log.Error($"OnCreateRoom: Unable to create game room.", ex);
                return;
            }

            room.Join(peer);
            Log.Debug($"OnCreateRoom: Created new room: {room.Number} and made the client to join it.");
        }

        protected override void OnJoinRoom(GamePeer peer, int roomId, string password, string clientVersion, string authToken, string magicHash)
        {
            /* Check the client version. */
            if (clientVersion != "4.7.1")
                peer.Disconnect();

            try
            {
                if (!peer.Authenticate(authToken, magicHash))
                {
                    peer.SendError();
                    return;
                }
            }
            catch
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "Failed to authenticate user. Try restarting UberStrike.");
                throw;
            }

            var room = GameApplication.Instance.Rooms.Get(roomId);
            if (room != null)
            {
                Log.Debug($"OnJoinRoom: Room: {roomId} IsPasswordProcted: {room.View.IsPasswordProtected}");

                /* Request password if the room is password protected & check password.*/
                if (room.View.IsPasswordProtected && password != room.Password)
                    peer.Events.SendRequestPasswordForRoom(room.View.Server.ConnectionString, room.Number);
                else
                    room.Join(peer);
            }
            else
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "Room does not exist anymore.");
                Log.Warn($"OnJoinRoom: Client wanted to join a room, but Rooms.Get returned null.");
            }
        }

        protected override void OnLeaveRoom(GamePeer peer)
        {
            if (peer.Room != null)
                peer.Room.Leave(peer);
            else
                Log.Error("A client tried to a leave a game room even though it was not in a room."); /* wtf fam? */
        }

        protected override void OnUpdatePing(GamePeer peer, ushort ping)
        {
            /* Ignore ping value sent by client, cause we use the server side only. */
            peer.Actor.Info.Ping = (ushort)(peer.RoundTripTime / 2);
        }

        protected override void OnUpdateKeyState(GamePeer peer, byte state)
        {
            // Space
        }

        protected override void OnUpdateLoadout(GamePeer peer)
        {
            peer.UpdateLoadout();
        }
    }
}