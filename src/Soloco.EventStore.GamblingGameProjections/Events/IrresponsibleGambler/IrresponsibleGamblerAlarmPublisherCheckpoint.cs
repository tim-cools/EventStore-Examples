namespace Soloco.EventStore.GamblingGameProjections.Events.IrresponsibleGambler
{
    public class IrresponsibleGamblerAlarmPublisherCheckpoint
    {
        public int LastEventProcessed { get; set; }

        public IrresponsibleGamblerAlarmPublisherCheckpoint(int eventNumber)
        {
            LastEventProcessed = eventNumber;
        }
    }
}