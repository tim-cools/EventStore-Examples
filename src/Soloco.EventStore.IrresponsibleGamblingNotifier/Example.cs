using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.GamblingGameProjections.Infrastructure;
using Soloco.EventStore.GamblingGameProjections.Projections;

namespace Soloco.EventStore.IrresponsibleGamblingNotifier
{
    internal class Example
    {
        private readonly IProjectionContext _projectionContext;
        private readonly GameOverToPlayerDistributorProjection _projection;

        private readonly EventReader _eventReader;

        private readonly GameSimulator _simulator;
        private readonly IConsole _console;

        public Example(IProjectionContext projectionContext, GameOverToPlayerDistributorProjection projection, EventReader eventReader, GameSimulator simulator, IConsole console)
        {
            _projectionContext = projectionContext;
            _projection = projection;
            _eventReader = eventReader;
            _simulator = simulator;
            _console = console;
        }

        public void Run()
        {
            EnsureProjections();

            _eventReader.StartReading();

            _simulator.Start();

            _console.ReadKey("Press any key to stop sample...");

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
            _simulator.Stop();
        }
    }
}