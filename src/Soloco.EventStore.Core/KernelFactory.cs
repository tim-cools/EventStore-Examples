using System.Reflection;
using Ninject;
using Ninject.Modules;

namespace Soloco.EventStore.Core
{
    public static class KernelFactory
    {
        public static T Get<T>(params Assembly[] assemblies)
        {
            var kernel = CreateKernel(assemblies);
            return kernel.Get<T>();
        }

        public static T Get<T>(INinjectModule module, params Assembly[] assemblies)
        {
            var kernel = CreateKernel(assemblies);
            kernel.Load(module);
            return kernel.Get<T>();
        }

        private static IKernel CreateKernel(Assembly[] assemblies)
        {
            var kernel = new StandardKernel();
            kernel.Load(new KernelModule(assemblies));
            return kernel;
        }
    }
}
