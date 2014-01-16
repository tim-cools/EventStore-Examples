using System;
using Soloco.EventStore.MeasurementProjections.Infrastructure;
using Soloco.EventStore.MeasurementProjections.Projections;

namespace Soloco.EventStore.MeasurementReadAveragePerDayCalculator
{
    internal class Example
    {
        private readonly IProjectionContext _projectionContext;
        private readonly MeasurementReadAveragePerDayProjection _projection;

        private readonly EventReader _eventReader;

        private readonly DeviceSimulator _deviceSimulator;
        private readonly IConsole _console;

        public Example(IProjectionContext projectionContext, MeasurementReadAveragePerDayProjection projection, EventReader eventReader, DeviceSimulator deviceSimulator, IConsole console)
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

            _deviceSimulator.Start(2, TimeSpan.FromMilliseconds(300));

            _console.ReadLine();

            Stop();
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

            _console.ReadLine();
        }
    }
}