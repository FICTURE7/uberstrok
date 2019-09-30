using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoapCore;
using System.ServiceModel;
using UberStrok.Core;
using UberStrok.WebServices.AspNetCore.Authentication;
using UberStrok.WebServices.AspNetCore.Authentication.Jwt;
using UberStrok.WebServices.AspNetCore.Configurations;
using UberStrok.WebServices.AspNetCore.Database;
using UberStrok.WebServices.AspNetCore.Database.LiteDb;

namespace UberStrok.WebServices.AspNetCore
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // Register configurations.
            services.Configure<MapsConfiguration>(Configuration.GetSection("Maps"));
            services.Configure<ItemsConfiguration>(Configuration.GetSection("Items"));
            services.Configure<ServersConfiguration>(Configuration.GetSection("Servers"));
            services.Configure<ApplicationConfiguration>(Configuration.GetSection("Application"));
            services.Configure<AccountConfiguration>(Configuration.GetSection("Account"));
            services.Configure<AuthConfiguration>(Configuration.GetSection("Auth"));

            // Register services.
            services.AddSingleton<IDbService, LiteDbService>();
            services.AddSingleton<IAuthService, JwtAuthService>();
            services.AddSingleton<ISessionService, SessionService>();

            services.AddSingleton<IFaultExceptionTransformer, DefaultFaultExceptionTransformer>();

            services.AddSingleton(s => new ItemManager(s.GetRequiredService<IOptions<ItemsConfiguration>>().Value));
            services.AddSingleton(s => new MapManager(s.GetRequiredService<IOptions<MapsConfiguration>>().Value));

            // Register web services.
            services.AddSingleton<ApplicationWebService>();
            services.AddSingleton<AuthenticationWebService>();
            services.AddSingleton<ShopWebService>();
            services.AddSingleton<UserWebService>();
            // services.AddSingleton<ClanWebService>();
            services.AddSingleton<PrivateMessageWebService>();
            services.AddSingleton<RelationshipWebService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSoapEndpoint<AuthenticationWebService>("/2.0/AuthenticationWebService", new BasicHttpBinding());
            app.UseSoapEndpoint<ApplicationWebService>("/2.0/ApplicationWebService", new BasicHttpBinding());
            app.UseSoapEndpoint<ShopWebService>("/2.0/ShopWebService", new BasicHttpBinding());
            app.UseSoapEndpoint<UserWebService>("/2.0/UserWebService", new BasicHttpBinding());
            // app.UseSoapEndpoint<ClanWebService>("/2.0/ClanWebService", new BasicHttpBinding());
            app.UseSoapEndpoint<PrivateMessageWebService>("/2.0/PrivateMessageWebService", new BasicHttpBinding());
            app.UseSoapEndpoint<RelationshipWebService>("/2.0/RelationshipWebService", new BasicHttpBinding());
        }
    }
}
