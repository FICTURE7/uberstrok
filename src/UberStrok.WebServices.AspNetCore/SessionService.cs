using System;
using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Authentication;
using UberStrok.WebServices.AspNetCore.Database;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public class SessionService : ISessionService
    {
        private readonly IDbService _database;
        private readonly IAuthService _auth;

        public SessionService(IDbService database, IAuthService auth)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        }

        public async Task<string> CreateSessionAsync(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            var createdAt = DateTime.UtcNow;
            var expiresAt = createdAt.AddHours(4);

            var session = new Session
            {
                Id = default,

                MemberId = member.Id,
                Creation = createdAt,
                Expiration = expiresAt,
            };

            member.LastLogin = createdAt;

            await _database.Sessions.InsertAsync(session);
            await _database.Members.UpdateAsync(member);

            return _auth.Create(member.Id, createdAt, expiresAt);
        }

        public async Task<Member> GetMemberAsync(string authToken)
        {
            if (authToken == null)
                throw new ArgumentNullException(nameof(authToken));

            var memberId = 0;
            var authError = AuthError.Ok;

            try { authError = _auth.TryGet(authToken, out memberId); }
            catch (Exception ex)
            {
                ThrowHelpers.ThrowSessionAuthThrew(ex);
            }

            if (authError != AuthError.Ok)
                ThrowHelpers.ThrowSessionAuthFailed(authError);

            var member = await _database.Members.FindAsync(memberId);
            if (member == null)
                ThrowHelpers.ThrowSessionMemberNotFound(memberId);
            if (member.IsBanned())
                ThrowHelpers.ThrowSessionMemberBanned(member);

            return member;
        }
    }
}
