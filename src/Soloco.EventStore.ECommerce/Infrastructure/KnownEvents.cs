using System;
using EventStore.ClientAPI;
using Soloco.EventStore.Core.Infrastructure;

namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public class KnownEventsProvider : IKnownEventsProvider
    {
        public KnownEvent Get(RecordedEvent recordedEvent)
        {
           
            return new KnownEvent(ConsoleColor.Yellow);
        }
    }
}