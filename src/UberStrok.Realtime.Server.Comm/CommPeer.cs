using log4net;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeer : BasePeer
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(CommPeer));

        public CommPeer(InitRequest request) : base(request)
        {
            /* Space */
        }
    }

    public abstract class BaseCommPeerOperationHandler : BaseOperationHandler<CommPeer>
    {
        public override int Id => 1;

        public abstract void OnAuthenticationRequest(string authToken, string magicHash);
        public abstract void OnSendHeartbeatResponse(string authToken, string responseHash);

        public override void OnOperationRequest(CommPeer peer, byte opCode, MemoryStream bytes)
        {
            var operation = (ICommPeerOperationsType)opCode;
            switch (operation)
            {
                case ICommPeerOperationsType.AuthenticationRequest:
                    AuthenticationRequest(bytes);
                    break;

                case ICommPeerOperationsType.SendHeartbeatResponse:
                    SendHeartbeatResponse(bytes);
                    break;
            }
        }

        private void AuthenticationRequest(MemoryStream bytes)
        {
            var authToken = StringProxy.Deserialize(bytes);
            var magicHash = StringProxy.Deserialize(bytes);

            OnAuthenticationRequest(authToken, magicHash);
        }

        private void SendHeartbeatResponse(MemoryStream bytes)
        {
            var authToken = StringProxy.Deserialize(bytes);
            var responseHash = StringProxy.Deserialize(bytes);

            OnSendHeartbeatResponse(authToken, responseHash);
        }
    }

    public abstract class BaseLobbyRoomOperationHandler : BaseOperationHandler<CommPeer>
    {
        protected abstract void OnFullPlayerListUpdate(CommPeer peer);
        protected abstract void OnUpdatePlayerRoom(CommPeer peer, GameRoomView room);
        protected abstract void OnResetPlayerRoom(CommPeer peer);
        protected abstract void OnUpdateFriendsList(CommPeer peer, int cmid);
        protected abstract void OnUpdateClanData(CommPeer peer, int cmid);
        protected abstract void OnUpdateInboxMessages(CommPeer peer, int cmid, int messageId);
        protected abstract void OnUpdateInboxRequests(CommPeer peer, int cmid);
        protected abstract void OnUpdateClanMembers(CommPeer peer, List<int> clanMembers);
        protected abstract void OnGetPlayersWithMatchingName(CommPeer peer, string search);
        protected abstract void OnChatMessageToAll(CommPeer peer, string message);
        protected abstract void OnChatMessageToPlayer(CommPeer peer, int cmid, string message);
        protected abstract void OnChatMessageToClan(CommPeer peer, List<int> clanMembers, string message);
        protected abstract void OnModerationMutePlayer(CommPeer peer, int durationInMinutes, int mutedCmid, bool disableChat);
        protected abstract void OnModerationPermanentBan(CommPeer peer, int cmid);
        protected abstract void OnModerationBanPlayer(CommPeer peer, int cmid);
        protected abstract void OnModerationKickGame(CommPeer peer, int cmid);
        protected abstract void OnModerationUnbanPlayer(CommPeer peer, int cmid);
        protected abstract void OnModerationCustomMessage(CommPeer peer, int cmid, string message);
        protected abstract void OnSpeedhackDetection(CommPeer peer);
        protected abstract void OnSpeedhackDetectionNew(CommPeer peer, List<float> timeDifferences);
        protected abstract void OnPlayersReported(CommPeer peer, List<int> cmids, int type, string details, string logs);
        protected abstract void OnUpdateNaughtyList(CommPeer peer);
        protected abstract void OnClearModeratorFlags(CommPeer peer, int cmid);
        protected abstract void OnSetContactList(CommPeer peer, List<int> cmids);
        protected abstract void OnUpdateAllActors(CommPeer peer);
        protected abstract void OnUpdateContacts(CommPeer peer);

        public override void OnOperationRequest(CommPeer peer, byte opCode, MemoryStream bytes)
        {
            var operation = (ILobbyRoomOperationsType)(opCode);
            switch (operation)
            {
                case ILobbyRoomOperationsType.FullPlayerListUpdate:
                    FullPlayerListUpdate(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.UpdatePlayerRoom:
                    UpdatePlayerRoom(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ResetPlayerRoom:
                    ResetPlayerRoom(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.UpdateFriendsList:
                    UpdateFriendsList(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.UpdateClanData:
                    UpdateClanData(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.UpdateInboxMessages:
                    UpdateInboxMessages(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.UpdateInboxRequests:
                    UpdateInboxRequests(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.UpdateClanMembers:
                    UpdateClanMembers(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.GetPlayersWithMatchingName:
                    GetPlayersWithMatchingName(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ChatMessageToAll:
                    ChatMessageToAll(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ChatMessageToPlayer:
                    ChatMessageToPlayer(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ChatMessageToClan:
                    ChatMessageToClan(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ModerationMutePlayer:
                    ModerationMutePlayer(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ModerationPermanentBan:
                    ModerationPermanentBan(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ModerationBanPlayer:
                    ModerationBanPlayer(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ModerationKickGame:
                    ModerationKickGame(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ModerationUnbanPlayer:
                    ModerationUnbanPlayer(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ModerationCustomMessage:
                    ModerationCustomMessage(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.SpeedhackDetection:
                    SpeedhackDetection(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.SpeedhackDetectionNew:
                    SpeedhackDetectionNew(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.PlayersReported:
                    PlayersReported(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.UpdateNaughtyList:
                    UpdateNaughtyList(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.ClearModeratorFlags:
                    ClearModeratorFlags(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.SetContactList:
                    SetContactList(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.UpdateAllActors:
                    UpdateAllActors(peer, bytes);
                    break;
                case ILobbyRoomOperationsType.UpdateContacts:
                    UpdateContacts(peer, bytes);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void FullPlayerListUpdate(CommPeer peer, MemoryStream bytes)
        {
            OnFullPlayerListUpdate(peer);
        }

        private void UpdatePlayerRoom(CommPeer peer, MemoryStream bytes)
        {
            var room = GameRoomViewProxy.Deserialize(bytes);
            OnUpdatePlayerRoom(peer, room);
        }

        private void ResetPlayerRoom(CommPeer peer, MemoryStream bytes)
        {
            OnResetPlayerRoom(peer);
        }

        private void UpdateFriendsList(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            OnUpdateFriendsList(peer, cmid);
        }

        private void UpdateClanData(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            OnUpdateClanData(peer, cmid);
        }

        private void UpdateInboxMessages(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            var messageId = Int32Proxy.Deserialize(bytes);
            OnUpdateInboxMessages(peer, cmid, messageId);
        }

        private void UpdateInboxRequests(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            OnUpdateInboxRequests(peer, cmid);
        }

        private void UpdateClanMembers(CommPeer peer, MemoryStream bytes)
        {
            var clanMembers = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
            OnUpdateClanMembers(peer, clanMembers);
        }

        private void GetPlayersWithMatchingName(CommPeer peer, MemoryStream bytes)
        {
            var search = StringProxy.Deserialize(bytes);
            OnGetPlayersWithMatchingName(peer, search);
        }

        private void ChatMessageToAll(CommPeer peer, MemoryStream bytes)
        {
            var message = StringProxy.Deserialize(bytes);
            OnChatMessageToAll(peer, message);
        }

        private void ChatMessageToPlayer(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            var message = StringProxy.Deserialize(bytes);
            OnChatMessageToPlayer(peer, cmid, message);
        }

        private void ChatMessageToClan(CommPeer peer, MemoryStream bytes)
        {
            var clanMembers = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
            var message = StringProxy.Deserialize(bytes);
            OnChatMessageToClan(peer, clanMembers, message);
        }

        private void ModerationMutePlayer(CommPeer peer, MemoryStream bytes)
        {
            var durationInMinutes = Int32Proxy.Deserialize(bytes);
            var mutedCmid = Int32Proxy.Deserialize(bytes);
            var disableChat = BooleanProxy.Deserialize(bytes);
            OnModerationMutePlayer(peer, durationInMinutes, mutedCmid, disableChat);
        }

        private void ModerationPermanentBan(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            OnModerationPermanentBan(peer, cmid);
        }

        private void ModerationBanPlayer(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            OnModerationBanPlayer(peer, cmid);
        }

        private void ModerationKickGame(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            OnModerationKickGame(peer, cmid);
        }

        private void ModerationUnbanPlayer(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            OnModerationUnbanPlayer(peer, cmid);
        }

        private void ModerationCustomMessage(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            var message = StringProxy.Deserialize(bytes);
            OnModerationCustomMessage(peer, cmid, message);
        }

        private void SpeedhackDetection(CommPeer peer, MemoryStream bytes)
        {
            OnSpeedhackDetection(peer);
        }

        private void SpeedhackDetectionNew(CommPeer peer, MemoryStream bytes)
        {
            var timeDifferences = ListProxy<float>.Deserialize(bytes, SingleProxy.Deserialize);
            OnSpeedhackDetectionNew(peer, timeDifferences);
        }

        private void PlayersReported(CommPeer peer, MemoryStream bytes)
        {
            var cmids = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
            var type = Int32Proxy.Deserialize(bytes);
            var details = StringProxy.Deserialize(bytes);
            var logs = StringProxy.Deserialize(bytes);
            OnPlayersReported(peer, cmids, type, details, logs);
        }

        private void UpdateNaughtyList(CommPeer peer, MemoryStream bytes)
        {
            OnUpdateNaughtyList(peer);
        }

        private void ClearModeratorFlags(CommPeer peer, MemoryStream bytes)
        {
            var cmid = Int32Proxy.Deserialize(bytes);
            OnClearModeratorFlags(peer, cmid);
        }

        private void SetContactList(CommPeer peer, MemoryStream bytes)
        {
            var cmids = ListProxy<int>.Deserialize(bytes, Int32Proxy.Deserialize);
            OnSetContactList(peer, cmids);
        }

        private void UpdateAllActors(CommPeer peer, MemoryStream bytes)
        {
            OnUpdateAllActors(peer);
        }

        private void UpdateContacts(CommPeer peer, MemoryStream bytes)
        {
            OnUpdateContacts(peer);
        }
    }
}
