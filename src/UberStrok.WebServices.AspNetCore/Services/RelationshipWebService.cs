using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Database;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public class RelationshipWebService : BaseRelationshipWebService
    {
        private static class Events
        {
            public static readonly EventId AcceptContactRequest = new EventId(5000, nameof(AcceptContactRequest));
            public static readonly EventId DeclineContactRequest = new EventId(5001, nameof(DeclineContactRequest));
            public static readonly EventId DeleteContact = new EventId(5002, nameof(DeleteContact));
            public static readonly EventId GetContactRequests = new EventId(5003, nameof(GetContactRequests));
            public static readonly EventId GetContactsByGroup = new EventId(5004, nameof(GetContactsByGroup));
            public static readonly EventId SendContactRequest = new EventId(5005, nameof(SendContactRequest));
        }

        private readonly IDbService _database;
        private readonly ISessionService _sessions;
        private readonly ILogger<RelationshipWebService> _logger;

        public RelationshipWebService(
            ILogger<RelationshipWebService> logger,
            IDbService database,
            ISessionService sessions)
        {
            _logger = logger;
            _database = database;
            _sessions = sessions;
        }

        public override async Task<PublicProfileView> AcceptContactRequest(string authToken, int contactRequestId)
        {
            var recvMember = await _sessions.GetMemberAsync(authToken);

            if (!recvMember.Socials.IncomingRequests.TryGetValue(contactRequestId, out ContactRequest request))
            {
                _logger.LogWarning(
                    Events.AcceptContactRequest,
                    "{recvMember} tried to accept contact request \"{contactRequestId}\", but contact request does not exist.",
                    recvMember,
                    contactRequestId);
                return null;
            }

            var sendMember = await _database.Members.FindAsync(request.SenderMemberId);
            if (sendMember == null)
            {
                _logger.LogWarning(
                    Events.DeclineContactRequest,
                    "{sendMember} tried to accept contact request \"{contactRequestId}\", but sender member does not exist.", 
                    recvMember, 
                    contactRequestId);
                return null;
            }

            sendMember.Socials.Contacts.Add(recvMember.Id);
            recvMember.Socials.Contacts.Add(sendMember.Id);

            request.Status = ContactRequestStatus.Accepted;

            await _database.Members.UpdateAsync(sendMember);
            await _database.Members.UpdateAsync(recvMember);

            return new PublicProfileView().From(sendMember);
        }

        public override async Task<bool> DeclineContactRequest(string authToken, int contactRequestId)
        {
            var recvMember = await _sessions.GetMemberAsync(authToken);

            if (!recvMember.Socials.IncomingRequests.TryGetValue(contactRequestId, out ContactRequest request))
            {
                _logger.LogWarning(
                    Events.DeclineContactRequest,
                    "{recvMember} tried to decline contact request \"{contactRequestId}\", but contact request does not exist.", 
                    recvMember, 
                    contactRequestId);
                return false;
            }
            else
            {
                request.Status = ContactRequestStatus.Refused;
                await _database.Members.UpdateAsync(recvMember);

                return true;
            }
        }

        public override async Task<MemberOperationResult> DeleteContact(string authToken, int contactCmid)
        {
            var member = await _sessions.GetMemberOrNullAsync(authToken);
            if (member == null)
                return MemberOperationResult.MemberNotFound;

            if (!member.Socials.Contacts.Contains(contactCmid))
            {
                _logger.LogWarning(
                    Events.DeleteContact,
                    "{member} tried to delete contact \"{contactCmid}\", but the contact is not in its contact list.", 
                    member,
                    contactCmid);
                return MemberOperationResult.InvalidData;
            }
            else
            {
                var contactMember = await _database.Members.FindAsync(contactCmid);
                if (contactMember == null)
                {
                    _logger.LogWarning(
                        Events.DeleteContact,
                        "{member} tried to delete contact with cmid {contactCmid}, but the contact does not exist.",
                        member,
                        contactCmid,
                        contactMember);

                    return MemberOperationResult.MemberNotFound;
                }
                else
                {
                    contactMember.Socials.Contacts.Remove(member.Id);
                    member.Socials.Contacts.Remove(contactMember.Id);

                    await _database.Members.UpdateAsync(contactMember);
                    await _database.Members.UpdateAsync(member);

                    return MemberOperationResult.Ok;
                }
            }
        }

        public override async Task<List<ContactRequestView>> GetContactRequests(string authToken)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            var requests = new List<ContactRequestView>();

            foreach (var kv in member.Socials.IncomingRequests)
            {
                var requestId = kv.Key;
                var request = kv.Value;

                if (request.Status != ContactRequestStatus.Pending)
                    continue;

                var senderMember = await _database.Members.FindAsync(request.SenderMemberId);
                if (senderMember == null)
                {
                    _logger.LogWarning(
                        Events.GetContactRequests,
                        "{sender} contains a contact request which points to sender member \"{senderMemberCmid}\", but the sender member does not exist.",
                        member,
                        request.SenderMemberId);
                }
                else
                {
                    requests.Add(new ContactRequestView
                    {
                        RequestId = requestId,
                        InitiatorCmid = senderMember.Id,
                        InitiatorName = senderMember.Name,
                        InitiatorMessage = request.TextContent,
                        ReceiverCmid = member.Id,
                        SentDate = request.Sent,
                        Status = request.Status
                    });
                }
            }

            return requests;
        }

        public override async Task<List<ContactGroupView>> GetContactsByGroups(string authToken, bool populateFacebook)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            var group = new ContactGroupView
            {
                Contacts = new List<PublicProfileView>(member.Socials.Contacts.Count),

                GroupId = default, // Not used.
                GroupName = default // Not used.
            };

            foreach (var contactCmid in member.Socials.Contacts)
            {
                var contactMember = await _database.Members.FindAsync(contactCmid);
                if (contactMember != null)
                {
                    group.Contacts.Add(new PublicProfileView().From(contactMember));
                }
                else
                {
                    _logger.LogWarning(
                        Events.GetContactsByGroup,
                        "{member} has a contact \"{contactCmid}\", but the contact member does not exist.",
                        member,
                        contactCmid);
                }
            }

            return new List<ContactGroupView> { group };
        }

        public override async Task SendContactRequest(string authToken, int receiverCmid, string message)
        {
            var sendMember = await _sessions.GetMemberAsync(authToken);
            var recvMember = await _database.Members.FindAsync(receiverCmid);
            if (recvMember == null)
            {
                _logger.LogWarning(
                    Events.SendContactRequest,
                    "{sendMember} tried to send contact request to member \"{receiverCmid}\", but the member does not exists.", 
                    sendMember,
                    receiverCmid);
            }
            else
            {
                // Check if member already sent a contact request to
                // receiverMember; if yes exit early.
                if (recvMember.Socials.IncomingRequests.Values.Any(x => x.SenderMemberId == sendMember.Id))
                {
                    _logger.LogDebug(
                        Events.SendContactRequest,
                        "{sendMember} tried to send contact request to member \"{recvMember}\", but already sent previously.",
                        sendMember,
                        recvMember);
                }
                else
                {
                    var request = new ContactRequest
                    {
                        Id = recvMember.Socials.IncomingRequests.Count,
                        SenderMemberId = sendMember.Id,
                        Sent = DateTime.UtcNow,
                        Status = ContactRequestStatus.Pending,
                        TextContent = message
                    };

                    recvMember.Socials.IncomingRequests.Add(request.Id, request);
                    await _database.Members.UpdateAsync(recvMember);
                }
            }
        }
    }
}
