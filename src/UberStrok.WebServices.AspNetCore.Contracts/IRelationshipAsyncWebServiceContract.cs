using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IRelationshipAsyncWebServiceContract
    {
        [OperationContract]
        Task<byte[]> SendContactRequest(byte[] data);

        [OperationContract]
        Task<byte[]> GetContactRequests(byte[] data);

        [OperationContract]
        Task<byte[]> AcceptContactRequest(byte[] data);

        [OperationContract]
        Task<byte[]> DeclineContactRequest(byte[] data);

        [OperationContract]
        Task<byte[]> DeleteContact(byte[] data);

        [OperationContract]
        Task<byte[]> GetContactsByGroups(byte[] data);
    }
}