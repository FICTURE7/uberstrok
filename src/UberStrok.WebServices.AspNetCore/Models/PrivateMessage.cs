using System;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class PrivateMessage
    {
        public int Id { get; set; }
        public int SenderMemberId { get; set; }
        public int ReceiverMemberId { get; set; }
        public DateTime Sent { get; set; }
        public string TextContent { get; set; }
        public bool ReceiverRead { get; set; }
    }
}
