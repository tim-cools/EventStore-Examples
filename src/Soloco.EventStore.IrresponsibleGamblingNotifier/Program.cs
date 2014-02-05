using Soloco.EventStore.Core;
using Soloco.EventStore.GamblingGameProjections;
using Soloco.EventStore.MeasurementReadAveragePerDayCalculator;

namespace Soloco.EventStore.IrresponsibleGamblingNotifier
{
    class Program
    {
        static void Main()
        {
            var example = KernelFactory.Get<Example>(GamblingGame.Assembly);

            example.Run();
        }
    }
}
