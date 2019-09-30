using System.Collections.Generic;

namespace UberStrok.Core.Views
{
    public class ContactGroupView
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<PublicProfileView> Contacts { get; set; }
    }
}
