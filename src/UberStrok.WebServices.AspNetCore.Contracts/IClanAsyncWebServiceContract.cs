using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IClanAsyncWebServiceContract
    {
        [OperationContract]
        Task<byte[]> GetOwnClan(byte[] data);

        [OperationContract]
        Task<byte[]> UpdateMemberPosition(byte[] data);

        [OperationContract]
        Task<byte[]> InviteMemberToJoinAGroup(byte[] data);

        [OperationContract]
        Task<byte[]> AcceptClanInvitation(byte[] data);

        [OperationContract]
        Task<byte[]> DeclineClanInvitation(byte[] data);

        [OperationContract]
        Task<byte[]> KickMemberFromClan(byte[] data);

        [OperationContract]
        Task<byte[]> DisbandGroup(byte[] data);

        [OperationContract]
        Task<byte[]> LeaveAClan(byte[] data);

        [OperationContract]
        Task<byte[]> GetMyClanId(byte[] data);

        [OperationContract]
        Task<byte[]> CancelInvitation(byte[] data);

        [OperationContract]
        Task<byte[]> GetAllGroupInvitations(byte[] data);

        [OperationContract]
        Task<byte[]> GetPendingGroupInvitations(byte[] data);

        [OperationContract]
        Task<byte[]> CreateClan(byte[] data);

        [OperationContract]
        Task<byte[]> TransferOwnership(byte[] data);

        [OperationContract]
        Task<byte[]> CanOwnAClan(byte[] data);
    }
}