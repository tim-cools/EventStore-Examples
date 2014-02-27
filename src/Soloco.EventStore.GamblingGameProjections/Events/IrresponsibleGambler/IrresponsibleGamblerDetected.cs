
using System;

namespace Soloco.EventStore.GamblingGameProjections.Events.IrresponsibleGambler
{
    public class IrresponsibleGamblerDetected
    {
        public string PlayerId { get; private set; }

        public int AmountSpentLast24Hours { get; private set; }

        public DateTime Timestamp { get; private set; }

        public IrresponsibleGamblerDetected(string playerId, int amountSpentLast24Hours, DateTime timestamp)
        {
            PlayerId = playerId;
            AmountSpentLast24Hours = amountSpentLast24Hours;
            Timestamp = timestamp;
        }
    }
}