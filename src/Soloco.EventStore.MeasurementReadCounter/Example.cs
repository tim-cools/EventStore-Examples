using System;
using Soloco.EventStore.Test.MeasurementProjections.Infrastructure;
using Soloco.EventStore.Test.MeasurementProjections.Projections;
using Soloco.EventStore.Test.MeasurementProjections.Queries;

namespace Soloco.EventStore.MeasurementProjections.MeasurmentReadCounter
{
    internal class Example
    {
        private readonly MeasurementReadCounterProjection _measurementReadCounterProjection;
        private readonly MeasurementReadCounterQuery _measurementReadCounterQuery;
        private readonly EventReader _eventReader;

        private readonly DeviceSimulator _deviceSimulator;
        private readonly IConsole _console;

        public Example(MeasurementReadCounterProjection measurementReadCounterProjection, MeasurementReadCounterQuery measurementReadCounterQuery, EventReader eventReader, DeviceSimulator deviceSimulator, IConsole console)
        {
            _measurementReadCounterProjection = measurementReadCounterProjection;
            _measurementReadCounterQuery = measurementReadCounterQuery;
            _eventReader = eventReader;
            _deviceSimulator = deviceSimulator;
            _console = console;
        }

        public void Run()
        {
            _measurementReadCounterProjection.Ensure();
            _eventReader.StartReading();

            ReadCounter();

            _deviceSimulator.Start(2, TimeSpan.FromSeconds(4));

            _console.ReadLine();

            Stop();
        }

        private void ReadCounter()
        {
            _measurementReadCounterQuery.SubscribeValueChange(ValueChanged);

            var measurementReadCounter = _measurementReadCounterQuery.GetValue();

            _console.Magenta("MeasurementReadCounter (init) : " + measurementReadCounter);
        }

        private void ValueChanged(MeasurementReadCounter counter)
        {
            _console.Magenta("MeasurementReadCounter: " + counter);
        }

        private void Stop()
        {
            _deviceSimulator.Stop();

            _console.ReadLine();
        }
    }
}