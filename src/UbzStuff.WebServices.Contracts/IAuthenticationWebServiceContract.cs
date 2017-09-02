using System.ServiceModel;

namespace UbzStuff.WebServices.Contracts
{
    [ServiceContract]
    public interface IAuthenticationWebServiceContract
    {
        [OperationContract]
        byte[] CreateUser(byte[] data);

        [OperationContract]
        byte[] CompleteAccount(byte[] data);

        [OperationContract]
        byte[] LoginMemberEmail(byte[] data);

        [OperationContract]
        byte[] LoginMemberFacebookUnitySdk(byte[] data);

        [OperationContract]
        byte[] LoginSteam(byte[] data);

        [OperationContract]
        byte[] LoginMemberPortal(byte[] data);

        [OperationContract]
        byte[] LinkSteamMember(byte[] data);
    }
}