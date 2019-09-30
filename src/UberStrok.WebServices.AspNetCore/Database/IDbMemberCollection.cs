using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore.Database
{
    public interface IDbMemberCollection : IDbCollection<Member>
    {
        Task<Member> FindAsync(int id, Member.LoadOptions options = Member.LoadOptions.All);
        Task<Member> FindAsync(string steamId, Member.LoadOptions options = Member.LoadOptions.All);
        Task<bool> SteamIdExists(string steamId);
        Task<bool> NameExists(string name);
    }
}
