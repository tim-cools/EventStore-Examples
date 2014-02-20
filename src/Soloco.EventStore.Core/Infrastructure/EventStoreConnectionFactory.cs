using EventStore.ClientAPI;

namespace Soloco.EventStore.Core.Infrastructure
{
    public static class EventStoreConnectionFactory
    {
        public static IEventStoreConnection Default()
        {
            var settings = ConnectionSettings.Create()
                .KeepReconnecting()
                .UseConsoleLogger();

            var connection = EventStoreConnection.Create(settings, IPEndPointFactory.DefaultTcp());            
            connection.Connect();
            return connection;
        }
    }
}