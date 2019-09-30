using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IUserAsyncWebServiceContract
    {
        [OperationContract]
        Task<byte[]> ChangeMemberName(byte[] data);

        [OperationContract]
        Task<byte[]> IsDuplicateMemberName(byte[] data);

        [OperationContract]
        Task<byte[]> GenerateNonDuplicatedMemberNames(byte[] data);

        [OperationContract]
        Task<byte[]> GetMemberWallet(byte[] data);

        [OperationContract]
        Task<byte[]> GetInventory(byte[] data);

        [OperationContract]
        Task<byte[]> GetCurrencyDeposits(byte[] data);

        [OperationContract]
        Task<byte[]> GetItemTransactions(byte[] data);

        [OperationContract]
        Task<byte[]> GetPointsDeposits(byte[] data);

        [OperationContract]
        Task<byte[]> GetLoadout(byte[] data);

        [OperationContract]
        Task<byte[]> GetLoadoutServer(byte[] data); // UberStrok extension.

        [OperationContract]
        Task<byte[]> SetLoadout(byte[] data);

        [OperationContract]
        Task<byte[]> GetMember(byte[] data);

        [OperationContract]
        Task<byte[]> GetMemberSessionData(byte[] data);

        [OperationContract]
        Task<byte[]> GetMemberListSessionData(byte[] data);
    }
}