using System;
using Soloco.EventStore.Core.Infrastructure;

namespace Soloco.EventStore.MeasurementProjections.Projections
{
    public class MeasurementReadAveragePerDayProjection
    {
        private readonly IProjectionContext _projectionsContext;

        public MeasurementReadAveragePerDayProjection(IProjectionContext projectionsContext)
        {
            if (projectionsContext == null) throw new ArgumentNullException("projectionsContext");

            _projectionsContext = projectionsContext;
        }

        public void Ensure()
        {
            var projectionSource = ProjectionSources.Read("MeasurementReadAveragePerDayCalculator");

            _projectionsContext.EnsureProjection("MeasurementReadAveragePerDayCalculator", projectionSource);
        }
    }
}