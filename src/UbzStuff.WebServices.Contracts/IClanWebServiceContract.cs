using System.ServiceModel;

namespace UbzStuff.WebServices.Contracts
{
    [ServiceContract]
    public interface IClanWebServiceContract
    {
        [OperationContract]
        byte[] GetOwnClan(byte[] data);

        [OperationContract]
        byte[] UpdateMemberPosition(byte[] data);

        [OperationContract]
        byte[] InviteMemberToJoinAGroup(byte[] data);

        [OperationContract]
        byte[] AcceptClanInvitation(byte[] data);

        [OperationContract]
        byte[] DeclineClanInvitation(byte[] data);

        [OperationContract]
        byte[] KickMemberFromClan(byte[] data);

        [OperationContract]
        byte[] DisbandGroup(byte[] data);

        [OperationContract]
        byte[] LeaveAClan(byte[] data);

        [OperationContract]
        byte[] GetMyClanId(byte[] data);

        [OperationContract]
        byte[] CancelInvitation(byte[] data);

        [OperationContract]
        byte[] GetAllGroupInvitations(byte[] data);

        [OperationContract]
        byte[] GetPendingGroupInvitations(byte[] data);

        [OperationContract]
        byte[] CreateClan(byte[] data);

        [OperationContract]
        byte[] TransferOwnership(byte[] data);

        [OperationContract]
        byte[] CanOwnAClan(byte[] data);
    }
}