using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IAuthenticationAsyncWebServiceContract
    {
        [OperationContract]
        Task<byte[]> CreateUser(byte[] data);

        [OperationContract]
        Task<byte[]> CompleteAccount(byte[] data);

        [OperationContract]
        Task<byte[]> LoginMemberEmail(byte[] data);

        [OperationContract]
        Task<byte[]> LoginMemberFacebookUnitySdk(byte[] data);

        [OperationContract]
        Task<byte[]> LoginSteam(byte[] data);

        [OperationContract]
        Task<byte[]> LoginMemberPortal(byte[] data);

        [OperationContract]
        Task<byte[]> LinkSteamMember(byte[] data);
    }
}