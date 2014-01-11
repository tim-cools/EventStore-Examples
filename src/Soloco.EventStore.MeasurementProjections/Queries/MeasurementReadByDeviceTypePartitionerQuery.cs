using System;
using EventStore.ClientAPI;
using Soloco.EventStore.MeasurementProjections.Infrastructure;

namespace Soloco.EventStore.MeasurementProjections.Queries
{
    public class MeasurementReadByDeviceTypePartitionerQuery
    {
        private readonly IProjectionContext _projectionContext;

        public MeasurementReadByDeviceTypePartitionerQuery(IEventStoreConnection connection, IProjectionContext projectionContext, IConsole console)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (console == null) throw new ArgumentNullException("console");

            _projectionContext = projectionContext;
        }

        public MeasurementReadCounter GetValue()
        {
            return _projectionContext.GetState<MeasurementReadCounter>("MeasurementReadByDeviceTypePartitioner");
        }
    }
}
