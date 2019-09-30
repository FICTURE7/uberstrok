using System.Collections.Generic;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class Clan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Motto { get; set; }
        public int LeaderId { get; set; }
        public ICollection<Member> Members { get; set; }
    }
}
