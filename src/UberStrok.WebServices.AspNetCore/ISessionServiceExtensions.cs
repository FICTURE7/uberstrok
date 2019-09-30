using System;
using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public static class ISessionServiceExtensions
    {
        public static async Task<Member> GetMemberOrNullAsync(this ISessionService sessions, string authToken)
        {
            try { return await sessions.GetMemberAsync(authToken); }
            catch (Exception)
            {
                // Something went wrong when getting member, return null.
                return null;
            }
        }
    }
}
