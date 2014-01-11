using System;

using EventStore.ClientAPI;
using Soloco.EventStore.MeasurementProjections.Events;

namespace Soloco.EventStore.MeasurementProjections.Infrastructure
{
    public class EventReader
    {
        private readonly IEventStoreConnection _connection;
        private readonly IConsole _console;

        public EventReader(IEventStoreConnection connection, IConsole console)
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
            var recordedEvent = data.Event;
            var linkedStream = data.Link != null ? data.Link.EventStreamId : null;

            if (IsSystemStream(linkedStream)) return;

            if (recordedEvent.EventType != "MeasurementRead") return;

            if (linkedStream == null)
            {
                _console.Green("MeasurementRead: {0} (stream: {1})",
                    recordedEvent.ParseJson<MeasurementRead>(), recordedEvent.EventStreamId);
            }
            else
            {
                _console.Log("MeasurementRead: {0} (link: {1})", recordedEvent.ParseJson<MeasurementRead>(),
                    linkedStream);
            }

            //if (recordedEvent.EventType == "MeasurementPeriod")
            //{
            //    {
            //        var @event = recordedEvent.ParseJson<MeasurementPeriod>();

            //        if (@event.Type == MeasurementPeriodType.Hour)
            //        {
            //            _console.Timings("Period Average Received: " + recordedEvent.EventStreamId + " | " + @event);
            //        }
            //        else if (@event.Type == MeasurementPeriodType.Days)
            //        {
            //            _console.Magenta("Period Average Received: " + recordedEvent.EventStreamId + " | " + @event);
            //        }
            //        else
            //        {
            //            _console.Cyan("Period Average Received: " + recordedEvent.EventStreamId + " | " + @event);
            //        }
            //    }
            //}
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