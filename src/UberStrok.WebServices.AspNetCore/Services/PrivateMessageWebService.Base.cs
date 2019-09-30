using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.AspNetCore
{
    public abstract class BasePrivateMessageWebService : IPrivateMessageAsyncWebServiceContract
    {
        public abstract Task DeleteThread(string authToken, int otherCmid);
        public abstract Task<List<MessageThreadView>> GetAllMessageThreadsForUser(string authToken, int pageIndex);
        public abstract Task<PrivateMessageView> GetMessageWithIdForCmid(string authToken, int messageId);
        public abstract Task<List<PrivateMessageView>> GetThreadMessages(string authToken, int otherCmid, int pageIndex);
        public abstract Task MarkThreadAsRead(string authToken, int otherCmid);
        public abstract Task<PrivateMessageView> SendMessage(string authToken, int receiverCmid, string content);

        async Task<byte[]> IPrivateMessageAsyncWebServiceContract.DeleteThread(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var otherCmid = Int32Proxy.Deserialize(bytes);
                await DeleteThread(authToken, otherCmid);
                return Array.Empty<byte>();
            }
        }

        async Task<byte[]> IPrivateMessageAsyncWebServiceContract.GetAllMessageThreadsForUser(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var pageIndex = Int32Proxy.Deserialize(bytes);
                var view = await GetAllMessageThreadsForUser(authToken, pageIndex);
                using (var outBytes = new MemoryStream())
                {
                    ListProxy<MessageThreadView>.Serialize(outBytes, view, MessageThreadViewProxy.Serialize);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IPrivateMessageAsyncWebServiceContract.GetMessageWithIdForCmid(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var messageId = Int32Proxy.Deserialize(bytes);
                var view = await GetMessageWithIdForCmid(authToken, messageId);
                using (var outBytes = new MemoryStream())
                {
                    PrivateMessageViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IPrivateMessageAsyncWebServiceContract.GetThreadMessages(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var otherCmid = Int32Proxy.Deserialize(bytes);
                var pageIndex = Int32Proxy.Deserialize(bytes);
                var view = await GetThreadMessages(authToken, otherCmid, pageIndex);
                using (var outBytes = new MemoryStream())
                {
                    ListProxy<PrivateMessageView>.Serialize(outBytes, view, PrivateMessageViewProxy.Serialize);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IPrivateMessageAsyncWebServiceContract.MarkThreadAsRead(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var otherCmid = Int32Proxy.Deserialize(bytes);
                await MarkThreadAsRead(authToken, otherCmid);
                return Array.Empty<byte>();
            }
        }

        async Task<byte[]> IPrivateMessageAsyncWebServiceContract.SendMessage(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var receiverCmid = Int32Proxy.Deserialize(bytes);
                var content = StringProxy.Deserialize(bytes);
                var view = await SendMessage(authToken, receiverCmid, content);
                using (var outBytes = new MemoryStream())
                {
                    PrivateMessageViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }
    }
}
