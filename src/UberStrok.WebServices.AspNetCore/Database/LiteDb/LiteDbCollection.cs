using System;
using System.Threading.Tasks;
using LiteDB;

namespace UberStrok.WebServices.AspNetCore.Database.LiteDb
{
    public abstract class LiteDbCollection<TDocument> : IDbCollection<TDocument> where TDocument : class
    {
        protected LiteDatabase Database { get; }
        protected LiteCollection<TDocument> Collection { get; }

        public LiteDbCollection(LiteDatabase db)
        {
            Database = db ?? throw new ArgumentNullException(nameof(db));
            Collection = db.GetCollection<TDocument>();
        }

        public abstract Task DeleteAsync(TDocument document);

        public virtual Task<TDocument> FindAsync(int id)
            => Task.FromResult(Collection.FindById(id));

        public virtual Task InsertAsync(TDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Collection.Insert(document);
            return Task.CompletedTask;
        }

        public virtual Task UpdateAsync(TDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Collection.Update(document);
            return Task.CompletedTask;
        }
    }
}
