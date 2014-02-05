using System.Reflection;
using Ninject;

namespace Soloco.EventStore.Core
{
    public static class KernelFactory
    {
        public static T Get<T>(params Assembly[] assemblies)
        {
            var kernel = CreateKernel(assemblies);
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
