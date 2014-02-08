using System;
using System.Collections;
using System.Collections.Generic;

namespace Soloco.EventStore.GamblingGameProjections.Events.Game
{
    public class GameOver
    {
        public string GameId { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public IEnumerable<GamePlayerResult> PlayerResults { get; private set; }

        public GameOver(string gameId, DateTime timeStamp, IEnumerable<GamePlayerResult> playerResults)
        {
            if (playerResults == null) throw new ArgumentNullException("playerResults");

            GameId = gameId;
            TimeStamp = timeStamp;
            PlayerResults = playerResults;
        }
    }
}