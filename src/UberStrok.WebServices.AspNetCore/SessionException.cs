using System;

namespace UberStrok.WebServices.AspNetCore
{
    public enum SessionError
    {
        Ok,
        TokenExpired,
        TokenInvalid,
        MemberIsBanned,
        MemberNotFound,
        Unknown
    }

    public class SessionException : Exception
    {
        public SessionError Error { get; }

        public SessionException(SessionError error) : this(error, null, null)
        {
            // Space
        }

        public SessionException(SessionError error, string message) : this(error, message, null)
        {
            // Space
        }

        public SessionException(SessionError error, string message, Exception innerException) : base(message, innerException)
        {
            if (error == SessionError.Ok)
                throw new ArgumentException(nameof(error));

            Error = error;
        }
    }
}
