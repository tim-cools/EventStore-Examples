
using System;

namespace Soloco.EventStore.GamblingGameProjections.Events.IrresponsibleGambler
{
    public class IrresponsibleGamblerDetected
    {
        public string PlayerId { get; private set; }

        public int AmountSpendLast24Hours { get; private set; }

        public DateTime Timestamp { get; private set; }

        public IrresponsibleGamblerDetected(string playerId, int amountSpendLast24Hours, DateTime timestamp)
        {
            PlayerId = playerId;
            AmountSpendLast24Hours = amountSpendLast24Hours;
            Timestamp = timestamp;
        }
    }
}