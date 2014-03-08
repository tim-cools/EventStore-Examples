using System.Reflection;
using EventStore.ClientAPI;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Ninject.Modules;
using Soloco.EventStore.Core.Infrastructure;
using Ninject.Extensions.Conventions;

namespace Soloco.EventStore.ECommerce
{
    public class KernelModule : NinjectModule
    {
        public override void Load()
        {
            var connectionManager = (IConnectionManager)GlobalHost.DependencyResolver.GetService(typeof(IConnectionManager));

            Kernel.Bind<IConnectionManager>()
                .ToConstant(connectionManager);
        }
    }
}
