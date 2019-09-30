using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IApplicationAsyncWebServiceContract
    {
        [OperationContract]
        Task<byte[]> AuthenticateApplication(byte[] data);

        [OperationContract]
        Task<byte[]> GetConfigurationData(byte[] data);

        [OperationContract]
        Task<byte[]> GetMaps(byte[] data);

        [OperationContract]
        Task<byte[]> SetMatchScore(byte[] data);
    }
}