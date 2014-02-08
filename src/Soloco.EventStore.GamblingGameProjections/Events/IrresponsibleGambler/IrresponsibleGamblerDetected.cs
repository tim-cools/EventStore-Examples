
namespace Soloco.EventStore.GamblingGameProjections.Events.IrresponsibleGambler
{
    public class IrresponsibleGamblerDetected
    {
        public string PlayerId { get; set; }
    
        public int AmountSpendLast24Hours { get; set; }
    }
}