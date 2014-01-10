using System;

using EventStore.ClientAPI;
using Soloco.EventStore.Test.MeasurementProjections.Events;

namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    public class EventReader
    {
        private readonly IEventStoreConnection _connection;
        private readonly IColorConsole _console;

        public EventReader(IEventStoreConnection connection, IColorConsole console)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (console == null) throw new ArgumentNullException("console");

            _connection = connection;
            _console = console;
        }

        public void StartReading()
        {
            _connection.SubscribeToAll(true, Appeared, Dropped, EventStoreCredentials.Default);
        }

        private void Appeared(EventStoreSubscription subscription, ResolvedEvent data)
        {
            // if (data.Link != null) return;

            var recordedEvent = data.Event;
            switch (recordedEvent.EventType)
            {
                case "MeasurementRead":
                    _console.Green("MeasurementRead: {0} (from: {1})", recordedEvent.ParseJson<MeasurementRead>(), recordedEvent.EventStreamId);
                    break;
                case "MeasurementPeriod":
                    {
                        var @event = recordedEvent.ParseJson<MeasurementPeriod>();

                        if (@event.Type == MeasurementPeriodType.Hour)
                        {
                            _console.Yellow("Period Average Received: " + recordedEvent.EventStreamId + " | " + @event);
                        }
                        else if (@event.Type == MeasurementPeriodType.Days)
                        {
                            _console.Magenta("Period Average Received: " + recordedEvent.EventStreamId + " | " + @event);
                        }
                        else
                        {
                            _console.Cyan("Period Average Received: " + recordedEvent.EventStreamId + " | " + @event);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void Dropped(EventStoreSubscription subscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
            var message = string.Format("Subscription {0} dropped: {1} (Recovery currently not implemented){2}{3}",
                subscription.StreamId, subscriptionDropReason, Environment.NewLine, exception);

            _console.Red(message);
        }
    }
}