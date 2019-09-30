using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Authentication;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    internal static class ThrowHelpers
    {
        public static void ThrowSessionAuthFailed(AuthError error)
        {
            Debug.Assert(error != AuthError.Ok);

            switch (error)
            {
                case AuthError.Expired:
                    throw new SessionException(SessionError.TokenExpired, "Session token expired.");
                case AuthError.InvalidSignature:
                    throw new SessionException(SessionError.TokenInvalid, "Session token is invalid.");

                default:
                    throw new SessionException(SessionError.Unknown, "AuthService failed to get member ID from session token.");
            }
        }

        public static void ThrowSessionAuthThrew(Exception innerException)
            => throw new SessionException(
                SessionError.Unknown,
                "IAuthService threw an Exception when trying to get member ID from session token. See inner exception.", 
                innerException);

        public static void ThrowSessionMemberBanned(Member member)
            => throw new SessionException(SessionError.MemberIsBanned, member.BanExpiration == DateTime.MaxValue ? 
                        $"Session pointed to member {member}, but the member is banned permanently." :
                        $"Session pointed to member {member}, but the member is banned until {member.BanExpiration.Value}.");

        public static void ThrowSessionMemberNotFound(int memberId)
            => throw new SessionException(SessionError.MemberNotFound, $"Session pointed to member {memberId}, but the member does not exist.");

        public static Task<byte[]> ThrowOperationNotSupported(string opName)
            => throw new NotSupportedException($"Operation \"{opName}\" is not used by the client version targeted by UberStrok; therefore it is not supported.");
    }
}
