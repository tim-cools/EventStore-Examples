using System;
using EventStore.ClientAPI;
using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.MeasurementProjections.Events;

namespace Soloco.EventStore.MeasurementProjections.Infrastructure
{
    public class KnownEvents : IKnownEventsProvider
    {
        public KnownEvent Get(RecordedEvent recordedEvent)
        {
            if (recordedEvent.EventType == "MeasurementRead")
            {
                return new KnownEvent<MeasurementRead>(ConsoleColor.Green, recordedEvent);
            }
            if (recordedEvent.EventType == "MeasurementAverageDay")
            {
                return new KnownEvent<MeasurementAverageDay>(ConsoleColor.Magenta, recordedEvent);
            }
            return new KnownEvent(ConsoleColor.Yellow);
        }
    }
}
