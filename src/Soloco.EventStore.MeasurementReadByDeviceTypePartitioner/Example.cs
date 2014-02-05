using System;
using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.MeasurementProjections.Infrastructure;
using Soloco.EventStore.MeasurementProjections.Projections;

namespace Soloco.EventStore.MeasurementReadByDeviceTypePartitioner
{
    internal class Example
    {
        private readonly IProjectionContext _projectionContext;
        private readonly MeasurementReadByDeviceTypePartitionerProjection _projection;

        private readonly EventReader _eventReader;

        private readonly DeviceSimulator _deviceSimulator;
        private readonly IConsole _console;

        public Example(IProjectionContext projectionContext, MeasurementReadByDeviceTypePartitionerProjection projection, EventReader eventReader, DeviceSimulator deviceSimulator, IConsole console)
        {
            _projectionContext = projectionContext;
            _projection = projection;
            _eventReader = eventReader;
            _deviceSimulator = deviceSimulator;
            _console = console;
        }

        public void Run()
        {
            EnsureProjections();

            _eventReader.StartReading();

            ConfigureDevices();

            _deviceSimulator.Start(4, TimeSpan.FromSeconds(1));

            _console.ReadKey();

            Stop();
        }

        private void ConfigureDevices()
        {
            _deviceSimulator.ConfigureDevice(0, "Fridge");
            _deviceSimulator.ConfigureDevice(1, "TV");

            _deviceSimulator.ConfigureDevice(2, "Fridge");
            _deviceSimulator.ConfigureDevice(3, "TV");
        }

        private void EnsureProjections()
        {
            _projectionContext.EnableProjection("$by_category");
            _projectionContext.EnableProjection("$stream_by_category");

            _projection.Ensure();
        }

        private void Stop()
        {
            _deviceSimulator.Stop();

            _console.ReadKey();
        }
    }
}