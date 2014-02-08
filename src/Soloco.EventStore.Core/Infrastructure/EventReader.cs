using System;
using EventStore.ClientAPI;

namespace Soloco.EventStore.Core.Infrastructure
{
    public class EventReader
    {
        private readonly IEventStoreConnection _connection;
        private readonly IConsole _console;
        private readonly IKnownEventsProvider _knownEventsProvider;

        public EventReader(IEventStoreConnection connection, IConsole console, IKnownEventsProvider knownEventsProvider)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (console == null) throw new ArgumentNullException("console");
            if (knownEventsProvider == null) throw new ArgumentNullException("knownEventsProvider");

            _connection = connection;
            _console = console;
            _knownEventsProvider = knownEventsProvider;
        }

        public void StartReading()
        {
            _connection.SubscribeToAll(true, Appeared, Dropped, EventStoreCredentials.Default);
        }

        private void Appeared(EventStoreSubscription subscription, ResolvedEvent data)
        {
            var recordedEvent = data.Event;
            if (IsSystemStream(recordedEvent.EventStreamId)) return;

            var linkedStream = data.Link != null ? data.Link.EventStreamId : null;
            if (IsSystemStream(linkedStream)) return;

            var eventDefinition = _knownEventsProvider.Get(recordedEvent);

            _console.Log(
                eventDefinition.Color,
                "{0}: {1} ({2})",
                recordedEvent.EventType,
                eventDefinition.Parse(),
                FormatStream(linkedStream, recordedEvent));
        }

        private static string FormatStream(string linkedStream, RecordedEvent recordedEvent)
        {
            return linkedStream == null
                ? "stream: " + recordedEvent.EventStreamId
                : "link: " + linkedStream;
        }

        private bool IsSystemStream(string linkedStream)
        {
            return linkedStream != null && linkedStream.StartsWith("$");
        }

        private void Dropped(EventStoreSubscription subscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
            var message = string.Format("Subscription {0} dropped: {1} (Recovery currently not implemented){2}{3}",
                subscription.StreamId, subscriptionDropReason, Environment.NewLine, exception);

            _console.Error(message);
        }
    }
}