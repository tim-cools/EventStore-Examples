using System;

namespace Soloco.EventStore.GamblingGameProjections.Events.Game
{
    public class GamePlayerResult
    {
        public string PlayerId { get; private set; }

        public int Amount { get; private set; }

        public GamePlayerResult(string playerId, int amount)
        {
            PlayerId = playerId;
            Amount = amount;
        }
    }
}