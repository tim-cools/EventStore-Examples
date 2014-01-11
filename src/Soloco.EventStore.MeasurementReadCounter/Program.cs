
using Soloco.EventStore.Test.MeasurementProjections;

namespace Soloco.EventStore.MeasurementProjections.MeasurmentReadCounter
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
