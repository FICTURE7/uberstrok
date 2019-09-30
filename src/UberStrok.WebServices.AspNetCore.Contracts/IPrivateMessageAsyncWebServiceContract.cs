using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IPrivateMessageAsyncWebServiceContract
    {
        [OperationContract]
        Task<byte[]> GetAllMessageThreadsForUser(byte[] data);

        [OperationContract]
        Task<byte[]> GetThreadMessages(byte[] data);

        [OperationContract]
        Task<byte[]> SendMessage(byte[] data);

        [OperationContract]
        Task<byte[]> GetMessageWithIdForCmid(byte[] data);

        [OperationContract]
        Task<byte[]> MarkThreadAsRead(byte[] data);

        [OperationContract]
        Task<byte[]> DeleteThread(byte[] data);
    }
}