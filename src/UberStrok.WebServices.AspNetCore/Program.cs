using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace UberStrok.WebServices.AspNetCore
{
    public class Program
    {
        public static void Main(string[] args)
            => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config => {
                    config.AddJsonFile("configs/game/items.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile("configs/game/maps.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile("configs/game/application.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile("configs/game/servers.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile("configs/account.json", optional: false, reloadOnChange: true);
                })
                .UseStartup<Startup>()
                .UseKestrel();
    }
}
