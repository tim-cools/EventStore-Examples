using Soloco.EventStore.MeasurementProjections;

namespace Soloco.EventStore.MeasurmentReadCounter
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
