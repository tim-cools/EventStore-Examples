using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Soloco.EventStore.Test.MeasurementProjections.Events;
using Soloco.EventStore.Test.MeasurementProjections.Infrastructure;

namespace Soloco.EventStore.Test.MeasurementProjections.Queries
{
    public class MeasurementReadCounterQuery
    {
        private const string ResultStreamName = "$projections-MeasurementReadCounter-result";

        private readonly IEventStoreConnection _connection;

        public MeasurementReadCounterQuery(IEventStoreConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            _connection = connection;
        }

        public MeasurementReadCounter GetValue()
        {
            var position = Position.End;
            AllEventsSlice slice;
            do
            {
                slice = _connection.ReadAllEventsBackward(position, 1, false, EventStoreCredentials.Default);
                var counter = ParseMeasurementReadCounter(slice);
                
                if (counter != null) return counter;

                position = slice.NextPosition;
            } while (slice.Events.Length > 0);
            
            return null;
        }

        private static MeasurementReadCounter ParseMeasurementReadCounter(AllEventsSlice slice)
        {
            var @event = slice.Events.FirstOrDefault(e => !e.Event.EventType.StartsWith("$") && e.OriginalEvent != null);

            return @event.OriginalEvent == null ? null : @event.ParseJson<MeasurementReadCounter>();
        }

        public void SubscribeValueChange(Action<MeasurementReadCounter> valueChanged)
        {
            _connection.SubscribeToStream(
                ResultStreamName, 
                false, 
                (e, r) => ValueChanged(e, r, valueChanged), Dropped, EventStoreCredentials.Default);
        }

        private void Dropped(EventStoreSubscription subscription, SubscriptionDropReason subscriptionDropReason, Exception exception)
        {
            Console.WriteLine("Subscription {0} dropped: {1} (No recovery implemented currently){2}{3}", subscription.StreamId, subscriptionDropReason, Environment.NewLine, exception);
        }

        private void ValueChanged(EventStoreSubscription eventStoreSubscription, ResolvedEvent resolvedEvent, Action<MeasurementReadCounter> valueChanged)
        {
            var value = resolvedEvent.ParseJson<MeasurementReadCounter>();
            valueChanged(value);
        }
    }
}
