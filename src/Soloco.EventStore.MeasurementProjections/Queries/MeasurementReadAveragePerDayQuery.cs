using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.MeasurementProjections.Infrastructure;
using Soloco.EventStore.MeasurementProjections.Events;

namespace Soloco.EventStore.MeasurementProjections.Queries
{
    public class MeasurementReadAveragePerDayQuery
    {
        private readonly IEventStoreConnection _connection;

        public MeasurementReadAveragePerDayQuery(IEventStoreConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            _connection = connection;
        }

        public IEnumerable<MeasurementReadAveragePerDay> GetValues(string deviceStreamName)
        {
            var projectionResultStream = "MeasurementAverageDay-" + deviceStreamName;

            return _connection.ReadStreamEventsBackward<MeasurementAverageDay>(projectionResultStream)
                .Select(e => new MeasurementReadAveragePerDay(e.Timeslot, e.Average))
                .ToList();        
        }
    }
}
