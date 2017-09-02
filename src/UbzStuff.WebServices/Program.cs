using System;
using System.Threading;

namespace UbzStuff.WebServices
{
    public static class Program
    {
        /*
            Login sequence:
            1. AuthenticateApplication
            2. LoginSteam -> may vary
            3. GetContactsByGroups
            4. GetConfigurationData
            5. GetMaps
            6. GetShop
            7. GetInventory
            8. GetLoadout
            9. GetMember
         */

        public static WebServiceManager Manager { get; set; }

        public static int Main(string[] args)
        {
            try
            {
                Manager = new WebServiceManager();
                Manager.Start();
            }
            catch
            {
                Console.WriteLine("Unable to start web services. Press any key to exit...");
                Console.ReadKey();
                return 1;
            }

            Thread.Sleep(Timeout.Infinite);
            return 0;
        }
    }
}
