
using System;

namespace Soloco.EventStore.GamblingGameProjections.Events.Player
{
    public class GameLost
    {
        public string PlayerId { get; private set; }

        public string GameId { get; private set; }

        public int Amount { get; private set; }

        public DateTime Timestamp { get; private set; }

        public GameLost(string playerId, string gameId, int amount, DateTime timestamp)
        {
            PlayerId = playerId;
            GameId = gameId;
            Amount = amount;
            Timestamp = timestamp;
        }
    }
}