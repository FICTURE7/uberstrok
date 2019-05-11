using log4net;
using System;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeerOperationHandler : BaseGamePeerOperationHandler
    {
        private readonly static List<int> EmptyList = new List<int>(0);
        private readonly static ILog Log = LogManager.GetLogger(nameof(GamePeerOperationHandler));
        private readonly static ILog ReportLog = LogManager.GetLogger("Report");

        private readonly static PhotonServerLoadView _loadView = new PhotonServerLoadView
        {
            /* TODO: Implement some configs or somethings. */
            MaxPlayerCount = 200,
            State = PhotonServerLoadView.Status.Alive,
        };

        protected override void OnSendHeartbeatResponse(GamePeer peer, string authToken, string responseHash)
        {
            try
            {
                if (!peer.HeartbeatCheck(responseHash))
                    peer.SendError();
            }
            catch (Exception ex)
            {
                Log.Error("Exception while checking heartbeat", ex);
                peer.SendError();
            }
        }

        protected override void OnGetGameListUpdates(GamePeer peer)
        {
            // TODO: Don't use that, cause apparently there is a FullGameList event which we can send to wipe stuff and things.
            // Clear the client list of games available.
            peer.Events.SendGameListUpdateEnd();

            var rooms = new List<GameRoomDataView>(GameApplication.Instance.Rooms.Count);
            foreach (var room in GameApplication.Instance.Rooms)
                rooms.Add(room.View);

            peer.Events.SendGameListUpdate(rooms, EmptyList);
        }

        protected override void OnGetServerLoad(GamePeer peer)
        {
            /* UberStrike does not care about this value, it uses its client side value. */
            _loadView.Latency = peer.RoundTripTime;
            _loadView.PeersConnected = GameApplication.Instance.PlayerCount;
            /* UberStrike also does not care about this value, it uses PeersConnected. */
            _loadView.PlayersConnected = GameApplication.Instance.PlayerCount;
            _loadView.RoomsCreated = GameApplication.Instance.Rooms.Count;
            _loadView.TimeStamp = DateTime.UtcNow;

            peer.Events.SendServerLoadData(_loadView);
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

            BaseGameRoom room;

            try
            {
                string webServices = GameApplication.Instance.Configuration.WebServices;
                room = GameApplication.Instance.Rooms.Create(roomData, password);
                room.Shop.Load(webServices, authToken);
            }
            catch (NotSupportedException)
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "UberStrok does not support the selected game mode.");
                return;
            }
            catch
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "Failed to create game room.");
                throw;
            }

            /* Set quick-switch flag. */
            room.View.GameFlags |= 4;

            try { room.Join(peer); }
            catch
            {
                room.Leave(peer);
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "Failed to join room.");
                throw;
            }
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

            BaseGameRoom room = GameApplication.Instance.Rooms.Get(roomId);
            if (room == null)
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "Room does not exist anymore.");
                return;
            }

            /* Request password if the room is password protected & check password.*/
            if (NeedPassword(room, peer.Member) && room.Password != password)
            {
                peer.Events.SendRequestPasswordForRoom(room.View.Server.ConnectionString, room.Number);
            }
            else
            {
                try { room.Join(peer); }
                catch
                {
                    room.Leave(peer);
                    peer.Events.SendRoomEnterFailed(string.Empty, 0, "Failed to join room.");
                    throw;
                }
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
            peer.Actor.Ping.Update(ping);

            if (peer.Actor.Ping.FalsePositive >= 25)
            {
                ReportLog.Warn($"[Ping] OnUpdatePing False positive reached {peer.Actor.Cmid}");
                peer.Disconnect();
                return;
            }

            peer.Actor.Info.Ping = (ushort)peer.Actor.Ping.Value;
        }

        protected override void OnUpdateKeyState(GamePeer peer, byte state)
        {
            // Space
        }

        protected override void OnUpdateLoadout(GamePeer peer)
        {
            try { peer.UpdateLoadout(); }
            catch
            {
                peer.Disconnect();
                throw;
            }
        }

        private bool NeedPassword(BaseGameRoom room, UberstrikeUserView user)
        {
            return room.View.IsPasswordProtected && user.CmuneMemberView.PublicProfile.AccessLevel <= MemberAccessLevel.Moderator;
        }
    }
}