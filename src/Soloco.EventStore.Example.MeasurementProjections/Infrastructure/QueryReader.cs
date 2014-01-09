using System;
using System.IO;
using System.Linq;
using System.Threading;
using EventStore.ClientAPI;
using Soloco.EventStore.Test.MeasurementProjections.Queries;

namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    public class QueryReader
    {
        private readonly MeasurementReadCounterQuery _measurementReadCounterQuery;

        public QueryReader(IEventStoreConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            _measurementReadCounterQuery = new MeasurementReadCounterQuery(connection);
        }

        public void Start()
        {
            var measurementReadCounter = _measurementReadCounterQuery.GetValue();
            Console.WriteLine("MeasurementReadCounter (init) : " + measurementReadCounter);

            _measurementReadCounterQuery.SubscribeValueChange(ValueChanged);
        }

        private static void ValueChanged(MeasurementReadCounter counter)
        {
            Console.WriteLine("MeasurementReadCounter: " + counter);
        }
    }
}