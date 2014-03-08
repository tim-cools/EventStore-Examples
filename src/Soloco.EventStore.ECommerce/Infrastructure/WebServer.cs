using System;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;
using Ninject;
using Owin;

namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public class WebServer
    {
        private readonly IKernel _kernel;

        private IDisposable _server;

        public WebServer(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void Start()
        {
            _server = WebApp.Start<WebServer>("http://localhost:8881");
            
            GlobalHost.DependencyResolver
                .Register(typeof(IHubActivator), () => new KernelHubActivator(_kernel));
        }

        public void Stop()
        {
            _server.Dispose();
        }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute("script", @"app/{folder}/{fileName}", new
            {
                controller = "Application"
            });

            config.Routes.MapHttpRoute("app", @"app/{fileName}", new
            {
                controller = "Application",
                folder = RouteParameter.Optional,
                fileName = RouteParameter.Optional
            });
            
            app.UseWebApi(config);

            app.MapSignalR();
        }
    }
}