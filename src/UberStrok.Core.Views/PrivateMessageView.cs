using System;

namespace UberStrok.Core.Views
{
    public class PrivateMessageView
    {
        public int PrivateMessageId { get; set; }
        public int FromCmid { get; set; }
        public string FromName { get; set; }
        public int ToCmid { get; set; }
        public DateTime DateSent { get; set; }
        public string ContentText { get; set; }
        public bool IsRead { get; set; }
        public bool HasAttachment { get; set; }
        public bool IsDeletedBySender { get; set; }
        public bool IsDeletedByReceiver { get; set; }
    }
}
