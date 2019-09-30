using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Database;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public class PrivateMessageWebService : BasePrivateMessageWebService
    {
        private readonly IDbService _database;
        private readonly ISessionService _sessions;
        private readonly ILogger<PrivateMessageWebService> _logger;

        public PrivateMessageWebService(ILogger<PrivateMessageWebService> logger, IDbService database, ISessionService sessions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _sessions = sessions ?? throw new ArgumentNullException(nameof(sessions));
        }

        public override async Task DeleteThread(string authToken, int otherCmid)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            throw new System.NotImplementedException();
        }

        public override async Task<List<MessageThreadView>> GetAllMessageThreadsForUser(string authToken, int pageIndex)
        {
            const int MESSAGE_PER_PAGE = 10;

            var member = await _sessions.GetMemberAsync(authToken);
            var messages = new List<MessageThreadView>(MESSAGE_PER_PAGE);

            return messages;
        }

        public override Task<PrivateMessageView> GetMessageWithIdForCmid(string authToken, int messageId)
        {
            throw new System.NotImplementedException();
        }

        public override Task<List<PrivateMessageView>> GetThreadMessages(string authToken, int otherCmid, int pageIndex)
        {
            throw new System.NotImplementedException();
        }

        public override async Task MarkThreadAsRead(string authToken, int otherCmid)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            throw new System.NotImplementedException();
        }

        public override async Task<PrivateMessageView> SendMessage(string authToken, int receiverCmid, string content)
        {
            var member = await _sessions.GetMemberAsync(authToken);
            var receiverMember = await _database.Members.FindAsync(receiverCmid);

            var message = new PrivateMessage
            {
                Id = default,

                SenderMemberId = member.Id,
                ReceiverMemberId = receiverMember.Id,
                ReceiverRead = false,
                Sent = DateTime.UtcNow,
                TextContent = content
            };

            // TODO: Implement.

            return new PrivateMessageView().From(message);
        }
    }
}
