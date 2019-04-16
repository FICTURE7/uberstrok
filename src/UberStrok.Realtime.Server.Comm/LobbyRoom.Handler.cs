using System;
using System.Collections.Generic;
using PhotonHostRuntimeInterfaces;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public partial class LobbyRoom
    {
        public override void OnDisconnect(CommPeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Log.Info($"{peer.Actor.Cmid} Disconnected {reasonCode} -> {reasonDetail}");
            Leave(peer);
        }

        protected override void OnChatMessageToAll(CommPeer peer, string message)
        {
            lock (Sync)
            {
                foreach (var otherPeer in Peers)
                {
                    if (otherPeer.Actor.Cmid != peer.Actor.Cmid)
                        otherPeer.Events.Lobby.SendLobbyChatMessage(peer.Actor.Cmid, peer.Actor.Name, message);
                }
            }
        }

        protected override void OnFullPlayerListUpdate(CommPeer peer)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdatePlayerRoom(CommPeer peer, GameRoomView room)
        {
            throw new NotImplementedException();
        }

        protected override void OnResetPlayerRoom(CommPeer peer)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateFriendsList(CommPeer peer, int cmid)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateClanData(CommPeer peer, int cmid)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateInboxMessages(CommPeer peer, int cmid, int messageId)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateInboxRequests(CommPeer peer, int cmid)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateClanMembers(CommPeer peer, List<int> clanMembers)
        {
            throw new NotImplementedException();
        }

        protected override void OnGetPlayersWithMatchingName(CommPeer peer, string search)
        {
            throw new NotImplementedException();
        }

        protected override void OnChatMessageToPlayer(CommPeer peer, int cmid, string message)
        {
            throw new NotImplementedException();
        }

        protected override void OnChatMessageToClan(CommPeer peer, List<int> clanMembers, string message)
        {
            throw new NotImplementedException();
        }

        protected override void OnModerationMutePlayer(CommPeer peer, int durationInMinutes, int mutedCmid, bool disableChat)
        {
            throw new NotImplementedException();
        }

        protected override void OnModerationPermanentBan(CommPeer peer, int cmid)
        {
            throw new NotImplementedException();
        }

        protected override void OnModerationBanPlayer(CommPeer peer, int cmid)
        {
            throw new NotImplementedException();
        }

        protected override void OnModerationKickGame(CommPeer peer, int cmid)
        {
            throw new NotImplementedException();
        }

        protected override void OnModerationUnbanPlayer(CommPeer peer, int cmid)
        {
            throw new NotImplementedException();
        }

        protected override void OnModerationCustomMessage(CommPeer peer, int cmid, string message)
        {
            throw new NotImplementedException();
        }

        protected override void OnSpeedhackDetection(CommPeer peer)
        {
            throw new NotImplementedException();
        }

        protected override void OnSpeedhackDetectionNew(CommPeer peer, List<float> timeDifferences)
        {
            if (IsSpeedHacking(timeDifferences))
                peer.SendError();
        }

        protected override void OnPlayersReported(CommPeer peer, List<int> cmids, int type, string details, string logs)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateNaughtyList(CommPeer peer)
        {
            throw new NotImplementedException();
        }

        protected override void OnClearModeratorFlags(CommPeer peer, int cmid)
        {
            throw new NotImplementedException();
        }

        protected override void OnSetContactList(CommPeer peer, List<int> cmids)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateAllActors(CommPeer peer)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateContacts(CommPeer peer)
        {
            throw new NotImplementedException();
        }
    }
}
