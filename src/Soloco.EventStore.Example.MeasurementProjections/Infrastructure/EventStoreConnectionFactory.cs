using EventStore.ClientAPI;

namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    public static class EventStoreConnectionFactory
    {
        public static IEventStoreConnection Default()
        {
            var connection = EventStoreConnection.Create(IPEndPointFactory.DefaultTcp());
            connection.Connect();
            return connection;
        }
    }
}