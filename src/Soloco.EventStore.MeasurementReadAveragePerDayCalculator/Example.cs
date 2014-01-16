using System;
using Soloco.EventStore.MeasurementProjections.Infrastructure;
using Soloco.EventStore.MeasurementProjections.Projections;
using Soloco.EventStore.MeasurementProjections.Queries;

namespace Soloco.EventStore.MeasurementReadAveragePerDayCalculator
{
    internal class Example
    {
        private readonly IProjectionContext _projectionContext;
        private readonly MeasurementReadAveragePerDayProjection _projection;
        private readonly MeasurementReadAveragePerDayQuery _query;

        private readonly EventReader _eventReader;

        private readonly DeviceSimulator _deviceSimulator;
        private readonly IConsole _console;

        public Example(IProjectionContext projectionContext, MeasurementReadAveragePerDayProjection projection, EventReader eventReader, DeviceSimulator deviceSimulator, IConsole console, MeasurementReadAveragePerDayQuery query)
        {
            _projectionContext = projectionContext;
            _projection = projection;
            _eventReader = eventReader;
            _deviceSimulator = deviceSimulator;
            _console = console;
            _query = query;
        }

        public void Run()
        {
            ShowAverages();

            EnsureProjections();

            _eventReader.StartReading();

            _deviceSimulator.Start(2, TimeSpan.FromMilliseconds(300));

            _console.ReadKey("Press any key to stop sample...");

            Stop();

            ShowAverages();
        }

        private void ShowAverages()
        {
            _console.Important("Get averages from store");

            ShowAverages("Device-0");
            ShowAverages("Device-1");

            _console.ReadKey("Press any key to continue...");
        }

        private void ShowAverages(string name)
        {
            var values = _query.GetValues(name);
            _console.Log("Averages: {0}", name);

            foreach (var value in values)
            {
                _console.Log("  - {0}", value);
            }
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
        }
    }
}