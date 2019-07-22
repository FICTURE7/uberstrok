using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class GamePeerOperationHandler : BaseGamePeerOperationHandler
    {
        public static readonly GamePeerOperationHandler Instance = new GamePeerOperationHandler();

        private readonly static ILog ReportLog = LogManager.GetLogger("Report");

        /* TODO: Implement some configs or somethings. */
        private readonly static PhotonServerLoadView _loadView = new PhotonServerLoadView
        {
            MaxPlayerCount = 200,
            State = PhotonServerLoadView.Status.Alive,

            /* 
             * UberStrike does not care about this value, it uses its client
             * side measurement.
             */
            Latency = default,
            /*
             * UberStrike also does not care about this value, it uses
             * PeersConnected.
             */
            PlayersConnected = default
        };

        protected override void OnSendHeartbeatResponse(GamePeer peer, string authToken, string responseHash)
        {
            try
            {
                if (!peer.HeartbeatCheck(responseHash))
                    peer.SendError();
            }
            catch
            {
                peer.SendError();
                throw;
            }
        }

        protected override void OnGetGameListUpdates(GamePeer peer)
        {
            var rooms = new List<GameRoomDataView>(GameApplication.Instance.Rooms.Count);
            foreach (var room in GameApplication.Instance.Rooms)
                rooms.Add(room.GetView());

            peer.Events.SendFullGameList(rooms);
        }

        protected override void OnGetServerLoad(GamePeer peer)
        {
            _loadView.PeersConnected = GameApplication.Instance.Lobby.Peers.Count;
            _loadView.RoomsCreated = GameApplication.Instance.Lobby.Rooms.Count;
            _loadView.TimeStamp = DateTime.UtcNow;

            peer.Events.SendServerLoadData(_loadView);
        }

        protected override void OnCreateRoom(GamePeer peer, GameRoomDataView roomData, string password, string clientVersion, string authToken, string magicHash)
        {
            /* Check the client version. */
            if (clientVersion != "4.7.1")
            {
                peer.Disconnect();
                return;
            }

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

            GameRoom room;
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

            Debug.Assert(room != null);

            /* Set quick-switch flag. */
            room.GetView().GameFlags |= 4;
            room.Join(peer);
        }

        protected override void OnJoinRoom(GamePeer peer, int roomId, string password, string clientVersion, string authToken, string magicHash)
        {
            /* Check the client version. */
            if (clientVersion != "4.7.1")
            {
                peer.Disconnect();
                return;
            }

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

            GameRoom room = GameApplication.Instance.Rooms.Get(roomId);
            if (room == null)
            {
                peer.Events.SendRoomEnterFailed(string.Empty, 0, "Room does not exist anymore.");
            }
            else
            {
                /* Request password if the room is password protected & check password.*/
                if (NeedPassword(room, peer.GetUser(false)) && !CheckPassword(room, password))
                    peer.Events.SendRequestPasswordForRoom(room.GetView().Server.ConnectionString, room.RoomId);
                else
                    room.Join(peer);
            }
        }

        protected override void OnLeaveRoom(GamePeer peer)
        {
            if (peer.Actor != null)
                peer.Actor.Room.Leave(peer);
            else
                /* wtf fam? */
                Log.Error("A client tried to a leave a game room even though it was not in a room.");
        }

        protected override void OnUpdatePing(GamePeer peer, ushort ping)
        {
            if (peer.Actor != null)
            {
                peer.Actor.Ping.Update(peer.RoundTripTime);

                if (peer.Actor.Ping.FalsePositive >= 25)
                {
                    ReportLog.Warn($"[Ping] OnUpdatePing False positive reached {peer.Actor.Cmid}");
                    peer.SendError();
                    peer.Ban();
                }
                else
                {
                    peer.Actor.Info.Ping = (ushort)peer.Actor.Ping.Value;
                }
            }
        }

        protected override void OnUpdateKeyState(GamePeer peer, byte state)
        {
            if (peer.Actor != null)
                peer.Actor.Movement.KeyState = state;
        }

        protected override void OnUpdateLoadout(GamePeer peer)
        {
            var actor = peer.Actor;
            if (actor == null)
            {
                Log.Error("Peer attempted to update loadout but was not associated with any Actor.");
            }
            else
            {
                try
                {
                    var shop = actor.Room.Shop;
                    var loadout = peer.GetLoadout(retrieve: true);

                    actor.Loadout.Update(shop, loadout);

                    actor.Info.Gear = actor.Loadout.Gear.GetAsList();
                    actor.Info.Weapons = actor.Loadout.Weapons.GetAsList();
                    actor.Info.QuickItems = actor.Loadout.QuickItems.GetAsList();

                    actor.Info.ArmorPointCapacity = actor.Loadout.Gear.GetArmorCapacity();
                }
                catch
                {
                    peer.Disconnect();
                    throw;
                }
            }
        }

        private static bool NeedPassword(GameRoom room, UberstrikeUserView user)
        {
            return room.GetView().IsPasswordProtected && user.CmuneMemberView.PublicProfile.AccessLevel <= MemberAccessLevel.Moderator;
        }

        private static bool CheckPassword(GameRoom room, string password)
        {
            return room.Password == password;
        }
    }
}