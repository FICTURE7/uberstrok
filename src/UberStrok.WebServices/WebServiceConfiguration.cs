using Newtonsoft.Json;
using System.Collections.Generic;

namespace UberStrok.WebServices
{
    public class WebServiceConfiguration
    {
        public static readonly WebServiceConfiguration Default = new WebServiceConfiguration
        {
            ServiceBase = "http://localhost/2.0",
            Wallet = new WalletConfiguration
            {
                StartingCredits = 10000,
                StartingPoints = 10000
            },
            Loadout = new LoadoutConfiguration
            {
                StartingItems = new List<int>
                {
                    1,
                    12
                }
            }
        };

        [JsonProperty("service_base")]
        public string ServiceBase { get; set; }

        [JsonProperty("wallet")]
        public WalletConfiguration Wallet { get; set; }

        [JsonProperty("loadout")]
        public LoadoutConfiguration Loadout { get; set; }

        public class WalletConfiguration
        {
            [JsonProperty("starting_points")]
            public int StartingPoints { get; set; }

            [JsonProperty("starting_credits")]
            public int StartingCredits { get; set; }
        }

        public class LoadoutConfiguration
        {
            [JsonProperty("starting_items")]
            public List<int> StartingItems { get; set; }
        }
    }
}
