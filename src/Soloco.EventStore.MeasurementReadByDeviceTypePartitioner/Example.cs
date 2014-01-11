using System;
using Soloco.EventStore.MeasurementProjections.Infrastructure;
using Soloco.EventStore.MeasurementProjections.Projections;
using Soloco.EventStore.MeasurementProjections.Queries;

namespace Soloco.EventStore.MeasurementReadByDeviceTypePartitioner
{
    internal class Example
    {
        private readonly MeasurementReadByDeviceTypePartitionerProjection _measurementReadCounterProjection;
        private readonly MeasurementReadByDeviceTypePartitionerQuery _measurementReadCounterQuery;
        private readonly EventReader _eventReader;

        private readonly DeviceSimulator _deviceSimulator;
        private readonly IConsole _console;

        public Example(MeasurementReadByDeviceTypePartitionerProjection measurementReadCounterProjection, MeasurementReadByDeviceTypePartitionerQuery measurementReadCounterQuery, EventReader eventReader, DeviceSimulator deviceSimulator, IConsole console)
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