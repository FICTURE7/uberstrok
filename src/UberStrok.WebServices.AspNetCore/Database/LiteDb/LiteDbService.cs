using LiteDB;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore.Database.LiteDb
{
    public class LiteDbService : IDbService
    {
        private readonly LiteDatabase _db;

        private readonly LiteDbSessionCollection _sessions;
        private readonly LiteDbClanCollection _clans;
        private readonly LiteDbMemberCollection _members;

        public IDbSessionCollection Sessions => _sessions;
        public IDbClanCollection Clans => _clans;
        public IDbMemberCollection Members => _members;

        public LiteDbService()
        {
            _db = new LiteDatabase("uberstrok.db");
            _db.Mapper.Entity<Member>()
                      .DbRef(m => m.Loadout)
                      .DbRef(m => m.Inventory)
                      .DbRef(m => m.Statistics)
                      .DbRef(m => m.Socials)
                      .DbRef(m => m.Transactions);

            _clans = new LiteDbClanCollection(_db);
            _members = new LiteDbMemberCollection(_db);
            _sessions = new LiteDbSessionCollection(_db);
        }
    }
}
