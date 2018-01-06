using System.ServiceModel;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IPrivateMessageWebServiceContract
    {
        [OperationContract]
        byte[] GetAllMessageThreadsForUser(byte[] data);

        [OperationContract]
        byte[] GetThreadMessages(byte[] data);

        [OperationContract]
        byte[] SendMessage(byte[] data);

        [OperationContract]
        byte[] GetMessageWithIdForCmid(byte[] data);

        [OperationContract]
        byte[] MarkThreadAsRead(byte[] data);

        [OperationContract]
        byte[] DeleteThread(byte[] data);
    }
}