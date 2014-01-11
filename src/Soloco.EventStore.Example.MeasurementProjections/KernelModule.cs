using EventStore.ClientAPI;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Soloco.EventStore.Test.MeasurementProjections.Infrastructure;

namespace Soloco.EventStore.Test.MeasurementProjections
{
    public class KernelModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(scanner => scanner
                .FromThisAssembly()
                .SelectAllClasses()
                .BindAllInterfaces()
                .Configure(c => c.InSingletonScope()));

            Kernel.Bind<IEventStoreConnection>()
                .ToMethod(context => EventStoreConnectionFactory.Default())
                .InSingletonScope();
        }
    }
}
