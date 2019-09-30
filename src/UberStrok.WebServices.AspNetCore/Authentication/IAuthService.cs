using System;

namespace UberStrok.WebServices.AspNetCore.Authentication
{

    public interface IAuthService
    {
        string Create(int memberId, DateTime createdAt, DateTime expiresAt);
        AuthError TryGet(string authToken, out int memberId);
    }
}