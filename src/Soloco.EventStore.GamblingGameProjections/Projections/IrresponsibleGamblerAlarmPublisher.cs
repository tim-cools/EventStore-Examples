using System;
using EventStore.ClientAPI;
using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.GamblingGameProjections.Events.IrresponsibleGambler;

namespace Soloco.EventStore.GamblingGameProjections.Projections
{
    public class IrresponsibleGamblerAlarmPublisher
    {
        const string StateStream = "IrresponsibleGamblingAlarmsPublishedState";
        const string AlarmStream = "IrresponsibleGamblingAlarms";
         
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly IBus _bus;

        public IrresponsibleGamblerAlarmPublisher(IEventStoreConnection eventStoreConnection, IBus bus)
        {
            if (eventStoreConnection == null) throw new ArgumentNullException("eventStoreConnection");
            if (bus == null) throw new ArgumentNullException("bus");

            _eventStoreConnection = eventStoreConnection;
            _bus = bus;
        }

        public void Start()
        {   
            var position = GetLastStatePosition(StateStream);

            _eventStoreConnection.SubscribeToStreamFrom(AlarmStream, position, true, Publish,
                userCredentials: EventStoreCredentials.Default);
        }

        private void Publish(EventStoreCatchUpSubscription subscribtion, ResolvedEvent resolvedEvent)
        {
            var alarm = resolvedEvent.ParseJson<IrresponsibleGamblerDetected>();

            _bus.Publish(alarm);

            UpdateState(resolvedEvent);
        }

        private void UpdateState(ResolvedEvent resolvedEvent)
        {
            var updatedState = new IrresponsibleGamblerNotifierState(resolvedEvent.Event.EventNumber)
                .AsJsonEvent();

            _eventStoreConnection.AppendToStream(StateStream, ExpectedVersion.Any, updatedState);
        }

        private int GetLastStatePosition(string stateStream)
        {
            var state = _eventStoreConnection.GetLastEvent<IrresponsibleGamblerNotifierState>(stateStream);
            return state != null ? state.LastEventProcessed : 0;
        }
    }
}