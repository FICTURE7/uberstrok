using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using PhotonHostRuntimeInterfaces;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGameRoom : BaseGameRoomOperationHandler, IRoom<GamePeer>
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(BaseGameRoom));

        private string _password;
        private readonly GameRoomDataView _data;
        private readonly List<GamePeer> _peers;
        private readonly List<GamePeer> _players;
        private readonly IReadOnlyCollection<GamePeer> _peersReadOnly;
        private readonly IReadOnlyCollection<GamePeer> _playersReadonly;

        public BaseGameRoom(GameRoomDataView data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _peers = new List<GamePeer>();
            _peersReadOnly = _peers.AsReadOnly();

            _players = new List<GamePeer>();
            _playersReadonly = _players.AsReadOnly();

            _data = data;
        }

        public override int Id => 0;
        public IReadOnlyCollection<GamePeer> Peers => _peersReadOnly;
        public IReadOnlyCollection<GamePeer> Players => _playersReadonly;

        public int Number
        {
            get { return _data.Number; }
            set { _data.Number = value; }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _data.IsPasswordProtected = !string.IsNullOrEmpty(value);
                _password = value;
            }
        }

        public GameRoomDataView Data => _data;

        public void Join(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peers.Add(peer);

            peer.Room = this;
            peer.Events.SendRoomEntered(Data);

            peer.AddOperationHandler(this);
        }

        public virtual void Leave(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _peers.Remove(peer);
            _players.Remove(peer);

            peer.Room = null;
            peer.RemoveOperationHandler(Id);

            _data.ConnectedPlayers = Players.Count;
        }

        public override void OnDisconnect(GamePeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Leave(peer);
        }

        protected override void OnChatMessage(GamePeer peer, string message, ChatContext context)
        {
            var cmid = peer.Member.CmuneMemberView.PublicProfile.Cmid;
            var name = peer.Member.CmuneMemberView.PublicProfile.Name;
            var accessLevel = peer.Member.CmuneMemberView.PublicProfile.AccessLevel;

            foreach (var opeer in Peers)
            {
                if (opeer.Member.CmuneMemberView.PublicProfile.Cmid != cmid)
                    opeer.Events.Game.SendChatMessage(cmid, name, message, accessLevel, context);
            }
        }

        protected override void OnJoinTeam(GamePeer peer, TeamID team)
        {
            _players.Add(peer);
            _data.ConnectedPlayers = Players.Count;

            /*
                Create the the gear list and weapons list.
                The client does not like it when its not of the proper size. 
             */
            var gear = new List<int>(7);
            var weapons = new List<int>(peer.Member.CmuneMemberView.MemberItems);

            for (int i = 0; i < 7; i++)
                gear.Add(0);

            var k = peer.Member.CmuneMemberView.MemberItems.Count;
            for (int i = 0; i < 4 - k; i++)
                weapons.Add(0);

            var actor = new GameActorInfoView
            {
                TeamID = team,
                Health = 100,
                PlayerId = (byte)Players.Count,
                Channel = ChannelType.Steam,
                PlayerState = PlayerStates.None,

                Cmid = peer.Member.CmuneMemberView.PublicProfile.Cmid,
                ClanTag = peer.Member.CmuneMemberView.PublicProfile.GroupTag,
                AccessLevel = peer.Member.CmuneMemberView.PublicProfile.AccessLevel,
                Ping = (ushort)(peer.RoundTripTime / 2),
                PlayerName = peer.Member.CmuneMemberView.PublicProfile.Name,
                Weapons = weapons,
                Gear = gear
            };
            peer.Actor = actor;

            /*
            foreach (var opeer in Peers)
                opeer.Events.Game.SendPlayerJoinGame(actor, new PlayerMovement());
            */

            s_log.Info($"Joining team -> CMID:{actor.Cmid}:{team}");
        }

        protected override void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes)
        {
            s_log.Debug($"Respawn Times -> {string.Join(", ", respawnTimes)}");
        }

        protected override void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Team: {team}");

            for (int i = 0; i < positions.Count; i++)
                builder.AppendLine($" - {positions[i]}: {rotations[i]}");

            s_log.Debug($"Spawn Points -> {builder}");
        }
    }
}
