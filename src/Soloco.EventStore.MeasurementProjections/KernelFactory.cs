using Ninject;

namespace Soloco.EventStore.MeasurementProjections
{
    public static class KernelFactory
    {
        public static T Get<T>()
        {
            var kernel = CreateKernel();
            return kernel.Get<T>();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(new KernelModule());
            return kernel;
        }
    }
}
