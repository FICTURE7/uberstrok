using log4net;
using System;
using System.ServiceModel;
using UbzStuff.WebServices.Contracts;

namespace UbzStuff.WebServices.Core
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class RelationshipWebService : BaseWebService, IRelationshipWebServiceContract
    {
        //TODO: Implement BaseRelationshipWebService.

        private readonly static ILog Log = LogManager.GetLogger(typeof(RelationshipWebService));

        public RelationshipWebService(WebServiceContext ctx) : base(ctx)
        {
            // Space
        }

        public byte[] AcceptContactRequest(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] DeclineContactRequest(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] DeleteContact(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] GetContactRequests(byte[] data)
        {
            //throw new NotImplementedException();
            return null;
        }

        public byte[] GetContactsByGroups(byte[] data)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to handle GetContactsByGroups request:");
                Log.Error(ex);
                return null;
            }
        }

        public byte[] SendContactRequest(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
