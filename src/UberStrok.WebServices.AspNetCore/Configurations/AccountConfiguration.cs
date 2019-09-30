using System.Collections.Generic;

namespace UberStrok.WebServices.AspNetCore.Configurations
{
    public class AccountConfiguration
    {
        public int Points { get; set; }
        public int Credits { get; set; }
        public List<string> Items { get; set; }
    }
}
