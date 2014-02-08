namespace Soloco.EventStore.Core.Infrastructure
{
    public interface IBus
    {
        void Publish<T>(T @event);
    }
}