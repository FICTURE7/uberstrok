using System;
using System.Collections.Generic;
using PhotonHostRuntimeInterfaces;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server.Comm
{
    public partial class LobbyRoom
    {
        public override void OnDisconnect(CommPeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Log.Info($"{peer.Actor.Cmid} Disconnected {reasonCode} -> {reasonDetail}");
            Leave(peer);
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

        private static readonly char[] _separators = { ' ' };

        protected override void OnChatMessageToAll(CommPeer peer, string message)
        {
            if (peer.Actor.IsMuted)
                return;

            lock (Sync)
            {
                if (peer.Actor.AccessLevel >= MemberAccessLevel.Moderator && message.Length > 0 && message[0] == '?')
                {
                    try
                    {
                        var args = message.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
                        var cmd = args[0];

                        string response;
                        switch (args[0])
                        {
                            case "?ban":
                                {
                                    if (args.Length < 2)
                                    {
                                        response = "Usage: ?ban <cmid>";
                                        break;
                                    }

                                    if (!int.TryParse(args[1], out int cmid))
                                    {
                                        response = "Error: <cmid> must be an integer.";
                                        break;
                                    }

                                    if (cmid == peer.Actor.Cmid)
                                    {
                                        response = "Banning yourself might be a bad idea. :)";
                                        break;
                                    }

                                    if (DoBan(peer, cmid))
                                        response = $"Banned user with CMID {cmid}.";
                                    else
                                        response = "Error: Failed to ban user.";
                                    break;
                                }

                            case "?unban":
                                {
                                    if (args.Length < 2)
                                    {
                                        response = "Usage: ?unban <cmid>";
                                        break;
                                    }

                                    if (!int.TryParse(args[1], out int cmid))
                                    {
                                        response = "Error: <cmid> must be an integer.";
                                        break;
                                    }

                                    if (cmid == peer.Actor.Cmid)
                                    {
                                        response = "You can't unban yourself.";
                                        break;
                                    }

                                    if (DoUnban(peer, cmid))
                                        response = $"Unbanned user with CMID {cmid}.";
                                    else
                                        response = "Error: Failed to unban user.";
                                    break;
                                }

                            case "?msg":
                                if (args.Length < 3)
                                {
                                    response = "Usage: ?msg <cmid>/all <message>";
                                    break;
                                }

                                int target = 0;
                                if (args[1] == "all") target = 0;
                                else
                                {
                                    if (!int.TryParse(args[1], out target))
                                    {
                                        response = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                }

                                var adminMessage = string.Join(" ", args, 2, args.Length - 2);
                                if (target == 0)
                                {
                                    if (peer.Actor.AccessLevel != MemberAccessLevel.Admin)
                                    {
                                        response = $"Error: Only admins can send to all users.";
                                        break;
                                    }

                                    lock (Sync)
                                    {
                                        foreach (var otherPeer in Peers)
                                            otherPeer.Events.Lobby.SendModerationCustomMessage(adminMessage);
                                    }

                                    response = $"Sent message to all users.";
                                }
                                else
                                {
                                    var otherPeer = Find(target);
                                    if (otherPeer != null)
                                    {
                                        otherPeer.Events.Lobby.SendModerationCustomMessage(adminMessage);
                                        response = $"Sent message to user with CMID {target}.";
                                    }
                                    else
                                        response = $"Could not find user with CMID {target} online.";
                                }
                                break;

                            default:
                                response = "Error: Unknown command.";
                                break;
                        }

                        peer.Events.Lobby.SendLobbyChatMessage(0, "Server", response);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Failed to handle command.", ex);
                        /* Fall through as normal message. */
                    }
                }

                foreach (var otherPeer in Peers)
                {
                    if (otherPeer.Actor.Cmid != peer.Actor.Cmid)
                        otherPeer.Events.Lobby.SendLobbyChatMessage(peer.Actor.Cmid, peer.Actor.Name, message);
                }
            }
        }

        protected override void OnChatMessageToPlayer(CommPeer peer, int cmid, string message)
        {
            if (peer.Actor.IsMuted)
                return;

            Find(cmid)?.Events.Lobby.SendPrivateChatMessage(peer.Actor.Cmid, peer.Actor.Name, message);
        }

        protected override void OnChatMessageToClan(CommPeer peer, List<int> clanMembers, string message)
        {
            throw new NotImplementedException();
        }

        protected override void OnModerationMutePlayer(CommPeer peer, int durationInMinutes, int mutedCmid, bool disableChat)
        {
            if (peer.Actor.AccessLevel < MemberAccessLevel.Moderator)
                return;

            var mutedPeer = Find(mutedCmid);
            if (mutedPeer != null && mutedPeer.Actor.AccessLevel < MemberAccessLevel.Moderator)
            {
                mutedPeer.Actor.IsMuted = durationInMinutes > 0;
                mutedPeer.Actor.MuteEndTime = DateTime.UtcNow.AddSeconds(durationInMinutes);
                mutedPeer.Events.Lobby.SendModerationMutePlayer(disableChat);
            }
        }

        protected override void OnModerationPermanentBan(CommPeer peer, int cmid)
        {
            /* NOTE: Not reachable from game client. */
        }

        protected override void OnModerationBanPlayer(CommPeer peer, int cmid)
        {
            if (peer.Actor.AccessLevel < MemberAccessLevel.SeniorQA)
                return;

            Find(cmid)?.SendError("You have been kicked from the game.");
        }

        protected override void OnModerationKickGame(CommPeer peer, int cmid)
        {
            if (peer.Actor.AccessLevel < MemberAccessLevel.Moderator)
                return;

            Find(cmid)?.Events.Lobby.SendModerationKickGame();
        }

        protected override void OnModerationUnbanPlayer(CommPeer peer, int cmid)
        {
            /* NOTE: Not reachable from game client. */
        }

        protected override void OnModerationCustomMessage(CommPeer peer, int cmid, string message)
        {
            if (peer.Actor.AccessLevel < MemberAccessLevel.Moderator)
                return;

            Find(cmid)?.Events.Lobby.SendModerationCustomMessage(message);
        }

        protected override void OnSpeedhackDetection(CommPeer peer)
        {
            /* NOTE: Not reachable from game client. */
            peer.SendError();
        }

        protected override void OnSpeedhackDetectionNew(CommPeer peer, List<float> timeDifferences)
        {
            /* NOTE: Not reachable from game client. */

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

        private bool DoBan(CommPeer peer, int cmid)
        {
            int code;
            try
            {
                var client = new ModerationWebServiceClient(CommApplication.Instance.Configuration.WebServices);
                code = client.BanCmid(peer.AuthToken, cmid);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to ban user.", ex);
                code = 1;
            }

            Find(cmid)?.SendError("You have been banned.");
            return code == 0;
        }

        private bool DoUnban(CommPeer peer, int cmid)
        {
            int code;
            try
            {
                var client = new ModerationWebServiceClient(CommApplication.Instance.Configuration.WebServices);
                code = client.UnbanCmid(peer.AuthToken, cmid);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to ban user.", ex);
                code = 1;
            }

            return code == 0;
        }
    }
}
