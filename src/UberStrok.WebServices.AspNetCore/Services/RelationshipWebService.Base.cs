using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.AspNetCore
{
    public abstract class BaseRelationshipWebService : IRelationshipAsyncWebServiceContract
    {
        public abstract Task<PublicProfileView> AcceptContactRequest(string authToken, int contactRequestId);
        public abstract Task<bool> DeclineContactRequest(string authToken, int contactRequestId);
        public abstract Task<MemberOperationResult> DeleteContact(string authToken, int contactCmid);
        public abstract Task<List<ContactRequestView>> GetContactRequests(string authToken);
        public abstract Task<List<ContactGroupView>> GetContactsByGroups(string authToken, bool populateFacebook);
        public abstract Task SendContactRequest(string authToken, int receiverCmid, string message);

        async Task<byte[]> IRelationshipAsyncWebServiceContract.AcceptContactRequest(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var contactRequestId = Int32Proxy.Deserialize(bytes);
                var view = await AcceptContactRequest(authToken, contactRequestId);
                using (var outBytes = new MemoryStream())
                {
                    PublicProfileViewProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IRelationshipAsyncWebServiceContract.DeclineContactRequest(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var contactRequestId = Int32Proxy.Deserialize(bytes);
                var view = await DeclineContactRequest(authToken, contactRequestId);
                using (var outBytes = new MemoryStream())
                {
                    BooleanProxy.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IRelationshipAsyncWebServiceContract.DeleteContact(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var contactCmid = Int32Proxy.Deserialize(bytes);
                var view = await DeleteContact(authToken, contactCmid);
                using (var outBytes = new MemoryStream())
                {
                    EnumProxy<MemberOperationResult>.Serialize(outBytes, view);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IRelationshipAsyncWebServiceContract.GetContactRequests(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var view = await GetContactRequests(authToken);
                using (var outBytes = new MemoryStream())
                {
                    ListProxy<ContactRequestView>.Serialize(outBytes, view, ContactRequestViewProxy.Serialize);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IRelationshipAsyncWebServiceContract.GetContactsByGroups(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var populateFacebook = BooleanProxy.Deserialize(bytes);
                var view = await GetContactsByGroups(authToken, populateFacebook);
                using (var outBytes = new MemoryStream())
                {
                    ListProxy<ContactGroupView>.Serialize(outBytes, view, ContactGroupViewProxy.Serialize);
                    return outBytes.ToArray();
                }
            }
        }

        async Task<byte[]> IRelationshipAsyncWebServiceContract.SendContactRequest(byte[] data)
        {
            using (var bytes = new MemoryStream(data))
            {
                var authToken = StringProxy.Deserialize(bytes);
                var receiverCmid = Int32Proxy.Deserialize(bytes);
                var message = StringProxy.Deserialize(bytes);
                await SendContactRequest(authToken, receiverCmid, message);
                return Array.Empty<byte>();
            }
        }
    }
}
