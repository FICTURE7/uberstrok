namespace UberStrok.WebServices.AspNetCore.Database.MongoDb
{
    public class MongoDbService : IDbService
    {
        public IDbSessionCollection Sessions => throw new System.NotImplementedException();
        public IDbClanCollection Clans => throw new System.NotImplementedException();
        public IDbMemberCollection Members => throw new System.NotImplementedException();
    }
}
