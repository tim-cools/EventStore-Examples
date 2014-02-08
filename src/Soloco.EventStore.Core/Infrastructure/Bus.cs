using System;

namespace Soloco.EventStore.Core.Infrastructure
{
    class Bus : IBus
    {
        private readonly IConsole _console;

        public Bus(IConsole console)
        {
            if (console == null) throw new ArgumentNullException("console");

            _console = console;
        }

        public void Publish<T>(T @event)
        {
            _console.Green("Domain event ({0}) published: {1}", @event.GetType().Name, @event);
        }
    }
}