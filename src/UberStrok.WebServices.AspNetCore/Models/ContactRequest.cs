using System;
using UberStrok.Core.Common;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class ContactRequest
    {
        public int Id { get; set; }
        public int SenderMemberId { get; set; }
        public DateTime Sent { get; set; }
        public string TextContent { get; set; }
        public ContactRequestStatus Status { get; set; }
    }
}
