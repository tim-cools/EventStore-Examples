using System.Reflection;
using EventStore.ClientAPI;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Soloco.EventStore.Core.Infrastructure;

namespace Soloco.EventStore.Core
{
    internal class KernelModule : NinjectModule
    {
        private readonly Assembly[] _assemblies;

        public KernelModule(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public override void Load()
        {
            LoadAssemblies(_assemblies);
            LoadAssemblies(GetType().Assembly);

            Kernel.Bind<IEventStoreConnection>()
                .ToMethod(context => EventStoreConnectionFactory.Default())
                .InSingletonScope();
        }

        private void LoadAssemblies(params Assembly[] assemblies)
        {
            if (assemblies == null) return;
            
            Kernel.Bind(scanner => scanner
                .From(assemblies)
                .SelectAllClasses()
                .BindAllInterfaces()
                .Configure(c => c.InSingletonScope()));
        }
    }
}
