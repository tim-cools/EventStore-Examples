using System;
using System.Linq;
using EventStore.ClientAPI;
using Soloco.EventStore.Test.MeasurementProjections.Events;
using Soloco.EventStore.Test.MeasurementProjections.Infrastructure;

namespace Soloco.EventStore.Test.MeasurementProjections.Queries
{
    public class MeasurementReadCounterQuery
    {
        private const string ResultStreamName = "$projections-MeasurementReadCounter-result";

        private readonly IColorConsole _console;
        private readonly IEventStoreConnection _connection;
        private readonly ProjectionContext _projectionContext;

        public MeasurementReadCounterQuery(IEventStoreConnection connection, ProjectionContext projectionContext, IColorConsole console)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (console == null) throw new ArgumentNullException("console");

            _connection = connection;
            _projectionContext = projectionContext;
            _console = console;
        }


        public MeasurementReadCounter GetValue()
        {
            //var streamMetadataResult = _connection.GetStreamMetadata(ResultStreamName, EventStoreCredentials.Default);
            //var events = _connection.ReadStreamEventsBackward(ResultStreamName, int.MaxValue, 1, false, EventStoreCredentials.Default);
            //if (events.Events.Length == 0) return null;

            return _projectionContext.GetState<MeasurementReadCounter>("MeasurementReadCounter");

            //var position = Position.End;
            //AllEventsSlice slice;
            //do
            //{
            //    slice = _connection.ReadAllEventsBackward(position, 1, false, EventStoreCredentials.Default);
            //    var counter = ParseMeasurementReadCounter(slice);
                
            //    if (counter != null) return counter;

            //    position = slice.NextPosition;
            //} while (slice.Events.Length > 0);
            
            //return null;
        }

        //private static MeasurementReadCounter ParseMeasurementReadCounter(AllEventsSlice slice)
        //{
        //    var @event = slice.Events.FirstOrDefault(e => !e.Event.EventType.StartsWith("$"));

        //    return @event.OriginalEvent == null ? null :;
        //}

        public void SubscribeValueChange(Action<MeasurementReadCounter> valueChanged)
        {
            _connection.SubscribeToStream(
                ResultStreamName, 
                false, 
                (e, r) => ValueChanged(e, r, valueChanged), Dropped, EventStoreCredentials.Default);
        }

        private void Dropped(EventStoreSubscription subscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
            _console.Red("Subscription {0} dropped: {1} (No recovery implemented currently){2}{3}", subscription.StreamId, subscriptionDropReason, Environment.NewLine, exception);
        }

        private void ValueChanged(EventStoreSubscription eventStoreSubscription, ResolvedEvent resolvedEvent, Action<MeasurementReadCounter> valueChanged)
        {
            var value = resolvedEvent.ParseJson<MeasurementReadCounter>();
            valueChanged(value);
        }
    }
}
