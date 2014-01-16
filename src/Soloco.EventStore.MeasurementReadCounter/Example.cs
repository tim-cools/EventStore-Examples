using System;
using Soloco.EventStore.MeasurementProjections.Infrastructure;
using Soloco.EventStore.MeasurementProjections.Projections;
using Soloco.EventStore.MeasurementProjections.Queries;

namespace Soloco.EventStore.MeasurementReadCounter
{
    internal class Example
    {
        private readonly MeasurementReadCounterProjection _projection;
        private readonly MeasurementReadCounterQuery _counterQuery;

        private readonly EventReader _eventReader;

        private readonly DeviceSimulator _deviceSimulator;
        private readonly IConsole _console;

        public Example(MeasurementReadCounterProjection projection, MeasurementReadCounterQuery counterQuery, EventReader eventReader, DeviceSimulator deviceSimulator, IConsole console)
        {
            _projection = projection;
            _counterQuery = counterQuery;
            _eventReader = eventReader;
            _deviceSimulator = deviceSimulator;
            _console = console;
        }

        public void Run()
        {
            _projection.Ensure();
            _eventReader.StartReading();

            ReadCounter();

            _deviceSimulator.Start(2, TimeSpan.FromSeconds(1));

            _console.ReadKey();

            Stop();
        }

        private void ReadCounter()
        {
            _counterQuery.SubscribeValueChange(ValueChanged);

            var measurementReadCounter = _counterQuery.GetValue();

            _console.Magenta("MeasurementReadCounter (init) : " + measurementReadCounter);
        }

        private void ValueChanged(MeasurementProjections.Queries.MeasurementReadCounter counter)
        {
            _console.Magenta("MeasurementReadCounter: " + counter);
        }

        private void Stop()
        {
            _deviceSimulator.Stop();

            _console.ReadKey();
        }
    }
}