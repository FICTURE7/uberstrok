using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using UbzStuff.WebServices.Core;

namespace UbzStuff.WebServices
{
    public class WebServiceCollection
    {
        public WebServiceCollection(WebServiceContext ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException(nameof(ctx));

            _ctx = ctx;

            _services = new List<BaseWebService>();
            _hosts = new List<ServiceHost>();

            _applicationService = new ApplicationWebService(_ctx);
            _authenticationService = new AuthenticationWebService(_ctx);
            _relationshipService = new RelationshipWebService(_ctx);
            _userService = new UserWebService(_ctx);
            _shopService = new ShopWebService(_ctx);

            _services.Add(_applicationService);
            _services.Add(_authenticationService);
            _services.Add(_relationshipService);
            _services.Add(_userService);
            _services.Add(_shopService);
        }

        public ApplicationWebService ApplicationWebService => _applicationService;
        public AuthenticationWebService AuthenticationWebService => _authenticationService;
        public RelationshipWebService RelationshipWebService => _relationshipService;
        public UserWebService UserWebService => _userService;
        public ShopWebService ShopWebService => _shopService;

        private readonly WebServiceContext _ctx;

        private readonly ApplicationWebService _applicationService;
        private readonly AuthenticationWebService _authenticationService;
        private readonly RelationshipWebService _relationshipService;
        private readonly UserWebService _userService;
        private readonly ShopWebService _shopService;

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

            if (contractType == null)
                throw new Exception("IMPOSSIBRU");

            //Log.Info($"Binding contract interface {contractInterface.Name}...");

            var builder = new UriBuilder(_ctx.ServiceBase);
            builder.Path = Path.Combine(builder.Path, name);

            var host = new ServiceHost(service);
            host.AddServiceEndpoint(contractType, binding, builder.Uri);

            _hosts.Add(host);
        }
    }
}
