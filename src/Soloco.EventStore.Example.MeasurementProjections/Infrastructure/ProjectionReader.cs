using System;
using System.Threading.Tasks;

using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Soloco.EventStore.Test.MeasurementProjections.Events;

namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    internal class ProjectionReader
    {
        private readonly IEventStoreConnection _connection;
        private readonly IMeasurementConsole _console;

        public ProjectionReader(IEventStoreConnection connection, IMeasurementConsole console)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (console == null) throw new ArgumentNullException("console");

            _connection = connection;
            _console = console;
        }

        public void Start()
        {
            Task.Run(() => StartAsync());
        }

        private void StartAsync()
        {
            _connection.SubscribeToAll(true, Appeared, Dropped, EventStoreCredentials.Default);
        }

        private void Appeared(EventStoreSubscription subscription, ResolvedEvent data)
        {
            var recordedEvent = data.Event;
            switch (recordedEvent.EventType)
            {
                case "MeasurementRead":
                    _console.Green("MeasurementRead: " + recordedEvent.ParseJson<MeasurementRead>());
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
            Console.WriteLine("Subscription {0} dropped: {1} (No recovery implemented currently){2}{3}", subscription.StreamId, subscriptionDropReason, Environment.NewLine, exception);
        }
    }
}