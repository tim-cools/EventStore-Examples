namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public interface IQueryDispatcher
    {
        TResult Execute<TResult>(IQuery<TResult> query);
    }
}