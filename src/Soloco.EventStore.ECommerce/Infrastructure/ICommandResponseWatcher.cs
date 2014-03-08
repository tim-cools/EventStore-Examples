using System;

namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public interface ICommandResponseWatcher
    {
        EventHandler<CommandResponseEventArgs> ResponseReceived { get; set; }
    }
}