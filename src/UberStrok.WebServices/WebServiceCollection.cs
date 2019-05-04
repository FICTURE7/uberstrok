using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using UberStrok.WebServices.Core;

namespace UberStrok.WebServices
{
    public class WebServiceCollection
    {
        public WebServiceCollection(WebServiceContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));

            _services = new List<BaseWebService>();
            _hosts = new List<ServiceHost>();

            ApplicationWebService = new ApplicationWebService(_ctx);
            AuthenticationWebService = new AuthenticationWebService(_ctx);
            RelationshipWebService = new RelationshipWebService(_ctx);
            UserWebService = new UserWebService(_ctx);
            ShopWebService = new ShopWebService(_ctx);
            ModerationWebService = new ModerationWebService(_ctx);

            _services.Add(ApplicationWebService);
            _services.Add(AuthenticationWebService);
            _services.Add(RelationshipWebService);
            _services.Add(UserWebService);
            _services.Add(ShopWebService);
            _services.Add(ModerationWebService);
        }

        public ApplicationWebService ApplicationWebService { get; }
        public AuthenticationWebService AuthenticationWebService { get; }
        public RelationshipWebService RelationshipWebService { get; }
        public UserWebService UserWebService { get; }
        public ShopWebService ShopWebService { get; }
        public ModerationWebService ModerationWebService { get; }

        private readonly WebServiceContext _ctx;
        private readonly List<BaseWebService> _services;
        private readonly List<ServiceHost> _hosts;

        public void Bind(Binding binding)
        {
            if (binding == null)
                throw new ArgumentNullException(nameof(binding));

            for (int i = 0; i < _services.Count; i++)
                InternalBind(binding, _services[i]);
        }

        public void Open()
        {
            for (int i = 0; i < _hosts.Count; i++)
                _hosts[i].Open();
        }

        public void Close()
        {
            for (int i = 0; i < _hosts.Count; i++)
                _hosts[i].Close();
        }

        private void InternalBind(Binding binding, BaseWebService service)
        {
            var type = service.GetType();
            var name = type.Name;
            var interfaceTypes = type.GetInterfaces();
            var contractType = (Type)null;

            foreach (var interfaceType in interfaceTypes)
            {
                var serviceAttribute = interfaceType.GetCustomAttribute<ServiceContractAttribute>();
                if (interfaceType != null)
                {
                    contractType = interfaceType;
                    break;
                }
            }

            // Should never happen.
            if (contractType == null)
                throw new Exception("Specified service did not have a ServiceContractAttribute.");

            //Log.Info($"Binding contract interface {contractInterface.Name}...");

            var builder = new UriBuilder(_ctx.ServiceBase);
            builder.Path = Path.Combine(builder.Path, name);

            var host = new ServiceHost(service);
            host.AddServiceEndpoint(contractType, binding, builder.Uri);

            _hosts.Add(host);
        }
    }
}
