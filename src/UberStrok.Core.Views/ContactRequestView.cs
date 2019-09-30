using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    public class ContactRequestView
    {
        public int RequestId { get; set; }
        public int InitiatorCmid { get; set; }
        public string InitiatorName { get; set; }
        public int ReceiverCmid { get; set; }
        public string InitiatorMessage { get; set; }
        public ContactRequestStatus Status { get; set; }
        public DateTime SentDate { get; set; }
    }
}
