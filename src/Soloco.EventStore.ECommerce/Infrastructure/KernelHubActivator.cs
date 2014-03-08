using Microsoft.AspNet.SignalR.Hubs;
using Ninject;

namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public class KernelHubActivator : IHubActivator
    {
        private readonly IKernel _container;

        public KernelHubActivator(IKernel kernel)
        {
            _container = kernel;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return _container.Get(descriptor.HubType) as IHub;
        }
    }
}