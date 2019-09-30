using System.Threading.Tasks;

namespace UberStrok.WebServices.AspNetCore.Database
{
    public interface IDbCollection<TDocument> where TDocument : class
    {
        Task<TDocument> FindAsync(int id);
        Task InsertAsync(TDocument document);
        Task UpdateAsync(TDocument document);
        Task DeleteAsync(TDocument document);
    }
}
