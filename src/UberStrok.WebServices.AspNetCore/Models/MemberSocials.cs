using System.Collections.Generic;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class MemberSocials
    {
        public IDictionary<int, ContactRequest> IncomingRequests { get; set; }
        public IList<int> Contacts { get; set; } // TODO: Update to ISet<int>.
    }
}
