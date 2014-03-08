namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public interface IHandleQuery<in TQuery, out TResult> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }
}