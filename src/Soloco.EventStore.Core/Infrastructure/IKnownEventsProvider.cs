using EventStore.ClientAPI;

namespace Soloco.EventStore.Core.Infrastructure
{
    public interface IKnownEventsProvider
    {
        KnownEvent Get(RecordedEvent recordedEvent);
    }
}