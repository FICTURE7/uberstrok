using System;
using System.Threading.Tasks;
using LiteDB;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore.Database.LiteDb
{
    public class LiteDbMemberCollection : LiteDbCollection<Member>, IDbMemberCollection
    {
        public LiteDbMemberCollection(LiteDatabase db) : base(db)
        {
            // Space
        }

        public override Task<Member> FindAsync(int id)
            => FindAsync(id, Member.LoadOptions.All);

        public Task<Member> FindAsync(int id, Member.LoadOptions options = Member.LoadOptions.All)
            => Task.FromResult(GetIncludes(options).FindById(id));

        public Task<Member> FindAsync(string steamId, Member.LoadOptions options = Member.LoadOptions.All)
            => Task.FromResult(GetIncludes(options).FindOne(m => m.SteamId == steamId));

        public Task<bool> NameExists(string name)
            => Task.FromResult(Collection.Exists(m => m.Name == name));

        public Task<bool> SteamIdExists(string steamId)
            => Task.FromResult(Collection.Exists(m => m.SteamId == steamId));

        public override Task DeleteAsync(Member document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Collection.Delete(document.Id);
            return Task.CompletedTask;
        }

        public override Task InsertAsync(Member document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            document.LastModify = DateTime.UtcNow;
            Collection.Insert(document);
            return Task.CompletedTask;
        }

        public override Task UpdateAsync(Member document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            // If our local copy is outdated; bail out.
            if (!Collection.Exists(x => x.Id == document.Id && x.LastModify == document.LastModify))
                throw new InvalidOperationException("Local Member object is outdated.");

            document.LastModify = DateTime.UtcNow;
            Collection.Update(document);
            return Task.CompletedTask;
        }

        private LiteCollection<Member> GetIncludes(Member.LoadOptions options)
        {
            var col = Collection;
            if (options.HasFlag(Member.LoadOptions.Transactions))
                col.Include(x => x.Transactions);
            if (options.HasFlag(Member.LoadOptions.Inventory))
                col.Include(x => x.Inventory);
            if (options.HasFlag(Member.LoadOptions.Loadout))
                col.Include(x => x.Loadout);
            if (options.HasFlag(Member.LoadOptions.Socials))
                col.Include(x => x.Socials);
            if (options.HasFlag(Member.LoadOptions.Statistics))
                col.Include(x => x.Statistics);

            return col;
        }
    }
}
