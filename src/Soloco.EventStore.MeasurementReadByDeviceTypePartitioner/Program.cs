using Soloco.EventStore.Test.MeasurementProjections;

namespace Soloco.EventStore.MeasurementReadByDeviceTypePartitioner
{
    class Program
    {
        static void Main()
        {
            var example = KernelFactory.Get<Example>();

            example.Run();
        }
    }
}
