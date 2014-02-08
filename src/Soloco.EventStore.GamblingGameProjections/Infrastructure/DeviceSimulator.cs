using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.GamblingGameProjections.Events.Game;

namespace Soloco.EventStore.GamblingGameProjections.Infrastructure
{
    public class GameSimulator
    {
        private const int NumberOfPlayers = 15;
        private const int EventIntervalMilliseconds = 3000;
        private const string GameStreamName = "Game-";

        private static readonly string[] PlayerIds = new string[NumberOfPlayers];
        private static readonly Random Random = new Random();

        private readonly IEventStoreConnection _connection;

        private volatile bool _running;

        static GameSimulator()
        {
            for (var player = 0; player < NumberOfPlayers; player++)
            {
                PlayerIds[player] = IdGenerator.New();
            }
        }

        public GameSimulator(IEventStoreConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            _connection = connection;
        }

        public void Start()
        {
            _running = true;

            Task.Run(() => StartAsync());
        }

        public void Stop()
        {
            _running = false;
        }

        private async void StartAsync()
        {
            var start = DateTime.Now;

            while (_running)
            {
                await AppendGameOverEvent(start);

                start = start.AddMinutes(60);

                Thread.Sleep(EventIntervalMilliseconds);
            }
        }

        private async Task AppendGameOverEvent(DateTime start)
        {
            var id = IdGenerator.New();

            var @event = new GameOver(id, start, RandomPlayers())
                .AsJsonEvent();

            var streamName = GameStreamName + id;

            await _connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, new[] {@event});
        }

        private IEnumerable<GamePlayerResult> RandomPlayers()
        {
            return RandomPlayerIds()
                .Select(playerId => new GamePlayerResult(playerId, RandomAmount()))
                .ToList();
        }

        private static int RandomAmount()
        {
            return Random.Next(0, 200) - 100;
        }

        private static IEnumerable<string> RandomPlayerIds()
        {
            var players = Random.Next(3, 9);
            var used = new List<int>();
            for (var player = 0; player < players; player++)
            {
                var index = GetRandomUnusedPlayerId(used);

                yield return PlayerIds[index];

                used.Add(index);
            }
        }

        private static int GetRandomUnusedPlayerId(ICollection<int> used)
        {
            var index = Random.Next(PlayerIds.Length);
            while (used.Contains(index))
            {
                index = Random.Next(PlayerIds.Length);
            }
            return index;
        }
    }
}