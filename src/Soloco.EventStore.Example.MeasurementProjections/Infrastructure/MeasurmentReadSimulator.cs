using System;
using System.Threading;
using System.Threading.Tasks;

using EventStore.ClientAPI;

using Soloco.EventStore.Test.MeasurementProjections.Events;

namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    internal class MeasurementReadSimulator
    {
        private readonly IEventStoreConnection _connection;
        private readonly MeasurementConsole _console;
        private volatile bool _running;

        public MeasurementReadSimulator(IEventStoreConnection connection, MeasurementConsole console)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            _connection = connection;
            _console = console;
        }

        public void Start()
        {
            _running = true;

            for (var i = 0; i < 1; i++)
            {
                var meter = i;
                Task.Run(() => StartAsync(meter));
            }
        }

        public void Stop()
        {
            _running = false;
        }

        private async void StartAsync(int meter)
        {
            var random = new Random();
            var streamName = "Meter-" + meter;

            await AppendConfiguredEvent(streamName);
             
            var start = DateTime.Now;
            var last = start;
            var i = 1;
            while(_running)
            {
                var @event = new MeasurementRead(start, random.Next(150, 300) / 10m)
                    .AsJson();

                await _connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, new[] { @event });

                start = start.AddMinutes(60);

                if (i % 100 == 0)
                {
                    _console.Red(streamName + ": 100 events duration ms " + (DateTime.Now - last).TotalMilliseconds);
                    last = DateTime.Now;
                }
                Thread.Sleep(5000);

                i++;
            }
        }

        private Task<WriteResult> AppendConfiguredEvent(string streamName)
        {
            var @event = new DeviceConfigured("Fridge", streamName)
                .AsJson();

            return _connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, new[] { @event });
        }
    }
}