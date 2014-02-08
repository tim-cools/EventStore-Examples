
using System;

namespace Soloco.EventStore.GamblingGameProjections.Events.Player
{
    public class GameLost
    {
        public Guid PlayerId { get; set; }

        public Guid GameId { get; set; }
        
        public int Amount { get; set; }
    }
}