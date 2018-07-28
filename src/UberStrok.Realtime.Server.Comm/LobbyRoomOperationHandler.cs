using System;
using System.Collections.Generic;
using log4net;
using PhotonHostRuntimeInterfaces;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class LobbyRoomOperationHandler : BaseLobbyRoomOperationHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommPeerOperationHandler).Name);

        protected override void OnChatMessageToAll(CommPeer peer, string message)
        {
            LobbyManager.Instance.ChatToAll(peer.Actor, message);
        }

        public override void OnDisconnect(CommPeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Log.Info($"{peer.Actor.Cmid} Disconnected {reasonCode} -> {reasonDetail}");

            // Remove the peer from the lobby list & update all peer's CommActor list.
            LobbyManager.Instance.Remove(peer.Actor.Cmid);
            LobbyManager.Instance.UpdateList();
        }

        protected override void OnFullPlayerListUpdate(CommPeer peer)
        {

        }

        protected override void OnUpdatePlayerRoom(CommPeer peer, GameRoomView room)
        {
            
        }

        protected override void OnResetPlayerRoom(CommPeer peer)
        {

        }

        protected override void OnUpdateFriendsList(CommPeer peer, int cmid)
        {

        }

        protected override void OnUpdateClanData(CommPeer peer, int cmid)
        {

        }

        protected override void OnUpdateInboxMessages(CommPeer peer, int cmid, int messageId)
        {

        }

        protected override void OnUpdateInboxRequests(CommPeer peer, int cmid)
        {

        }

        protected override void OnUpdateClanMembers(CommPeer peer, List<int> clanMembers)
        {

        }

        protected override void OnGetPlayersWithMatchingName(CommPeer peer, string search)
        {

        }

        protected override void OnChatMessageToPlayer(CommPeer peer, int cmid, string message)
        {

        }

        protected override void OnChatMessageToClan(CommPeer peer, List<int> clanMembers, string message)
        {

        }

        protected override void OnModerationMutePlayer(CommPeer peer, int durationInMinutes, int mutedCmid, bool disableChat)
        {

        }

        protected override void OnModerationPermanentBan(CommPeer peer, int cmid)
        {

        }

        protected override void OnModerationBanPlayer(CommPeer peer, int cmid)
        {

        }

        protected override void OnModerationKickGame(CommPeer peer, int cmid)
        {

        }

        protected override void OnModerationUnbanPlayer(CommPeer peer, int cmid)
        {

        }

        protected override void OnModerationCustomMessage(CommPeer peer, int cmid, string message)
        {

        }

        protected override void OnSpeedhackDetection(CommPeer peer)
        {

        }

        protected override void OnSpeedhackDetectionNew(CommPeer peer, List<float> timeDifferences)
        {

        }

        protected override void OnPlayersReported(CommPeer peer, List<int> cmids, int type, string details, string logs)
        {

        }

        protected override void OnUpdateNaughtyList(CommPeer peer)
        {

        }

        protected override void OnClearModeratorFlags(CommPeer peer, int cmid)
        {

        }

        protected override void OnSetContactList(CommPeer peer, List<int> cmids)
        {

        }

        protected override void OnUpdateAllActors(CommPeer peer)
        {

        }

        protected override void OnUpdateContacts(CommPeer peer)
        {

        }
    }
}
