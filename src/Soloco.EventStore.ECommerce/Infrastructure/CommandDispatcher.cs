using EventStore.ClientAPI;
using Soloco.EventStore.Core.Infrastructure;

namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IEventStoreConnection _connection;

        public CommandDispatcher(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public void Execute(string order, string id, ICommand command)
        {
            var streamName = string.Format("{0}-{1}", order, id);

            _connection.AppendToStream(streamName, 
                ExpectedVersion.Any , 
                new [] { command.AsJsonEvent() }, 
                EventStoreCredentials.Default);
        }
    }
}