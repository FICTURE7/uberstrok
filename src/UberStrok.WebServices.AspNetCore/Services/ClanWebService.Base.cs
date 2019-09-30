using System.Threading.Tasks;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.AspNetCore
{
    public abstract class BaseClanWebService : IClanAsyncWebServiceContract
    {
        Task<byte[]> IClanAsyncWebServiceContract.AcceptClanInvitation(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.CancelInvitation(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.CanOwnAClan(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.CreateClan(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.DeclineClanInvitation(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.DisbandGroup(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.GetAllGroupInvitations(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.GetMyClanId(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.GetOwnClan(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.GetPendingGroupInvitations(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.InviteMemberToJoinAGroup(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.KickMemberFromClan(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.LeaveAClan(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.TransferOwnership(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        Task<byte[]> IClanAsyncWebServiceContract.UpdateMemberPosition(byte[] data)
        {
            throw new System.NotImplementedException();
        }
    }
}
