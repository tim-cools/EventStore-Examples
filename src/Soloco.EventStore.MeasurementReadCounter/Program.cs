using Soloco.EventStore.Core;
using Soloco.EventStore.MeasurementProjections;

namespace Soloco.EventStore.MeasurementReadCounter
{
    class Program
    {
        static void Main()
        {
            var example = KernelFactory.Get<Example>(Measurements.Assembly);

            example.Run();
        }
    }
}
