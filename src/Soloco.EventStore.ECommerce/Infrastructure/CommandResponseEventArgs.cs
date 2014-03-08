namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public class CommandResponseEventArgs
    {
        public CommandResponse Response { get; private set; }

        public CommandResponseEventArgs(CommandResponse response)
        {
            Response = response;
        }
    }
}