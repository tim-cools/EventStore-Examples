using System;
using Soloco.EventStore.Core.Infrastructure;

namespace Soloco.EventStore.MeasurementProjections.Projections
{
    public class MeasurementReadCounterProjection
    {
        private readonly IProjectionContext _projectionsContext;

        public MeasurementReadCounterProjection(IProjectionContext projectionsContext)
        {
            if (projectionsContext == null) throw new ArgumentNullException("projectionsContext");

            _projectionsContext = projectionsContext;
        }

        public void Ensure()
        {
            var projectionSource = ProjectionSources.Read("MeasurementReadCounter");

            _projectionsContext.EnsureProjection("MeasurementReadCounter", projectionSource);
        }
    }
}