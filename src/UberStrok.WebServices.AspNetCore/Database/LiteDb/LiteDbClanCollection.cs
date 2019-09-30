using System;
using System.Threading.Tasks;
using LiteDB;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore.Database.LiteDb
{
    public class LiteDbClanCollection : LiteDbCollection<Clan>, IDbClanCollection
    {
        public LiteDbClanCollection(LiteDatabase db) : base(db)
        {
            // Space
        }

        public override Task DeleteAsync(Clan document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Collection.Delete(document.Id);
            return Task.CompletedTask;
        }
    }
}
