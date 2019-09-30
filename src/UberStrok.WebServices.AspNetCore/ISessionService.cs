using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore
{
    public interface ISessionService
    {
        Task<string> CreateSessionAsync(Member member);
        Task<Member> GetMemberAsync(string authToken);
    }
}