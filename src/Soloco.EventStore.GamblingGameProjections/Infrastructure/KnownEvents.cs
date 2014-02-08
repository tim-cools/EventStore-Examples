using System;
using EventStore.ClientAPI;
using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.GamblingGameProjections.Events.Game;
using Soloco.EventStore.GamblingGameProjections.Events.Player;

namespace Soloco.EventStore.GamblingGameProjections.Infrastructure
{
    public class KnownEventsProvider : IKnownEventsProvider
    {
        public KnownEvent Get(RecordedEvent recordedEvent)
        {
            if (recordedEvent.EventType == "GameOver")
            {
                return new KnownEvent<GameOver>(ConsoleColor.Green, recordedEvent);
            }
            if (recordedEvent.EventType == "GameWon")
            {
                return new KnownEvent<GameWon>(ConsoleColor.Magenta, recordedEvent);
            }
            if (recordedEvent.EventType == "GameLost")
            {
                return new KnownEvent<GameLost>(ConsoleColor.Magenta, recordedEvent);
            }
            return new KnownEvent(ConsoleColor.Yellow);
        }
    }
}