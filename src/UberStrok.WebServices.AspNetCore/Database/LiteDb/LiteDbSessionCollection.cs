using System;
using System.Threading.Tasks;
using LiteDB;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore.Database.LiteDb
{
    public class LiteDbSessionCollection : LiteDbCollection<Session>, IDbSessionCollection
    {
        public LiteDbSessionCollection(LiteDatabase db) : base(db)
        {
            // Space
        }

        public override Task<Session> FindAsync(int id)
            => Task.FromResult(Collection.FindById(id.ToString()));

        public override Task DeleteAsync(Session document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Collection.Delete(document.Id);
            return Task.CompletedTask;
        }
    }
}
