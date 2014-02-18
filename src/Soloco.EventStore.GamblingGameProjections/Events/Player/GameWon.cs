
using System;

namespace Soloco.EventStore.GamblingGameProjections.Events.Player
{
    public class GameWon
    {
        public string PlayerId { get; set; }

        public string GameId { get; set; }

        public int Amount { get; set; }
        
        public DateTime Timestamp { get; private set; }
    }
}