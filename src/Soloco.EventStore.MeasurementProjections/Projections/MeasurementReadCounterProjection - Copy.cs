using System;
using Soloco.EventStore.Test.MeasurementProjections.Infrastructure;

namespace Soloco.EventStore.Test.MeasurementProjections.Projections
{
    public class MeasurementReadByDeviceTypePartitionerProjection
    {
        private readonly IProjectionContext _projectionsContext;

        public MeasurementReadByDeviceTypePartitionerProjection(IProjectionContext projectionsContext)
        {
            if (projectionsContext == null) throw new ArgumentNullException("projectionsContext");

            _projectionsContext = projectionsContext;
        }

        public void Ensure()
        {
            var projectionSource = ProjectionSources.Read("MeasurementReadByDeviceTypePartitioner");

            _projectionsContext.EnsureProjection("MeasurementReadByDeviceTypePartitioner", projectionSource);
        }
    }
}