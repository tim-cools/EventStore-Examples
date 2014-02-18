using System;
using System.Collections;
using System.Collections.Generic;

namespace Soloco.EventStore.GamblingGameProjections.Events.Game
{
    public class GameOver
    {
        public string GameId { get; private set; }

        public DateTime Timestamp { get; private set; }

        public IEnumerable<GamePlayerResult> PlayerResults { get; private set; }

        public GameOver(string gameId, DateTime timestamp, IEnumerable<GamePlayerResult> playerResults)
        {
            if (gameId == null) throw new ArgumentNullException("gameId");
            if (playerResults == null) throw new ArgumentNullException("playerResults");

            GameId = gameId;
            Timestamp = timestamp;
            PlayerResults = playerResults;
        }
    }
}