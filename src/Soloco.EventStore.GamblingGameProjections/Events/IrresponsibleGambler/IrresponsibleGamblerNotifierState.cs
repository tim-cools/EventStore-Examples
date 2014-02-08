namespace Soloco.EventStore.GamblingGameProjections.Events.IrresponsibleGambler
{
    public class IrresponsibleGamblerNotifierState
    {
        public int LastEventProcessed { get; set; }

        public IrresponsibleGamblerNotifierState(int eventNumber)
        {
            LastEventProcessed = eventNumber;
        }
    }
}