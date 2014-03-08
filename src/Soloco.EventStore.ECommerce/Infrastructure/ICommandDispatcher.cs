namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public interface ICommandDispatcher
    {
        void Execute(string type, string id, ICommand command); 
    }
}