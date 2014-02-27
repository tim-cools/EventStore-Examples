using System;
using EventStore.ClientAPI;
using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.GamblingGameProjections.Events.IrresponsibleGambler;

namespace Soloco.EventStore.GamblingGameProjections.Projections
{
    public class IrresponsibleGamblerAlarmPublisher
    {
        private const string CheckpointStream = "$publisher-IrresponsibleGamblerAlarmPublisher-checkpoint";
        private const string AlarmStream = "IrresponsibleGamblingAlarms";
         
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly IBus _bus;
        private readonly IConsole _console;

        private bool _running;

        public IrresponsibleGamblerAlarmPublisher(IEventStoreConnection eventStoreConnection, IBus bus, IConsole console)
        {
            if (eventStoreConnection == null) throw new ArgumentNullException("eventStoreConnection");
            if (bus == null) throw new ArgumentNullException("bus");
            if (console == null) throw new ArgumentNullException("console");

            _eventStoreConnection = eventStoreConnection;
            _bus = bus;
            _console = console;
        }

        public void Start()
        {
            if (_running) throw new InvalidOperationException("Projection already running");

            _running = true;

            Connect();
        }

        public void Stop()
        {
            if (!_running) throw new InvalidOperationException("Projection not running");

            _running = false;
        }

        private void Connect()
        {
            var position = GetLastCheckpoint(CheckpointStream);

            _eventStoreConnection.SubscribeToStreamFrom(AlarmStream, position, true, ProcessEvent,
                userCredentials: EventStoreCredentials.Default, subscriptionDropped: TryToReconnect);
        }

        private void TryToReconnect(EventStoreCatchUpSubscription catchUpSubscription, SubscriptionDropReason reason, Exception exception)
        {
            _console.Error("Projection subscription dropped: " + reason, exception);

            Connect();
        }   

        private void ProcessEvent(EventStoreCatchUpSubscription subscribtion, ResolvedEvent resolvedEvent)
        {
            var alarm = resolvedEvent.ParseJson<IrresponsibleGamblerDetected>();

            Publish(alarm);

            StoreCheckpoint(resolvedEvent);
        }

        private void Publish(IrresponsibleGamblerDetected alarm)
        {
            _bus.Publish(alarm);
        }

        private void StoreCheckpoint(ResolvedEvent resolvedEvent)
        {
            var eventNumber = resolvedEvent.Event.EventNumber;
            var checkpoint = new IrresponsibleGamblerAlarmPublisherCheckpoint(eventNumber)
                .AsJsonEvent();

            SetCheckpointStreamMaxCount(eventNumber);

            _eventStoreConnection.AppendToStream(CheckpointStream, ExpectedVersion.Any, EventStoreCredentials.Default, checkpoint);
        }

        private void SetCheckpointStreamMaxCount(int eventNumber)
        {
            if (eventNumber != 0) return;

            var metadata = StreamMetadata.Build().SetMaxCount(1);

            _eventStoreConnection.SetStreamMetadata(CheckpointStream, ExpectedVersion.Any, metadata, EventStoreCredentials.Default);
        }

        private int? GetLastCheckpoint(string stateStream)
        {
            var state = _eventStoreConnection.GetLastEvent<IrresponsibleGamblerAlarmPublisherCheckpoint>(stateStream);
            return state != null ? state.LastEventProcessed : (int?) null;
        }
    }
}