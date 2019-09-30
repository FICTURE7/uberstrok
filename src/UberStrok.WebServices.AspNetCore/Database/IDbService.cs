namespace UberStrok.WebServices.AspNetCore.Database
{
    public interface IDbService
    {
        IDbSessionCollection Sessions { get; }
        IDbClanCollection Clans { get; }
        IDbMemberCollection Members { get; }
        /*
        IDbMemberCollection Contacts { get; }
        IDbMemberCollection ContactRequests { get; }
        IDbMemberCollection Loadouts { get; }
        IDbMemberCollection Statistics { get; }
        IDbMemberCollection Threads { get; }
        */
    }
}
