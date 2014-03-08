using System;
using EventStore.ClientAPI;
using Soloco.EventStore.Core.Infrastructure;

namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public class CommandResponseWatcher : ICommandResponseWatcher
    {
        private readonly IEventStoreConnection _connection;

        public EventHandler<CommandResponseEventArgs> ResponseReceived { get; set; }

        public CommandResponseWatcher(IEventStoreConnection connection)
        {
            _connection = connection;

            Subsribe();
        }

        private void Subsribe()
        {
            _connection.SubscribeToStream("OrderCommandResponses", false, CommandResponseReceived, SubscriptionDropped);
        }

        private void CommandResponseReceived(EventStoreSubscription subscription, ResolvedEvent resolvedEvent)
        {
            if (ResponseReceived == null) return;

            var response = resolvedEvent.ParseJson<CommandResponse>();

            ResponseReceived(this, new CommandResponseEventArgs(response));
        }

        private void SubscriptionDropped(EventStoreSubscription arg1, SubscriptionDropReason arg2, Exception arg3)
        {
            Subsribe();
        }
    }
}