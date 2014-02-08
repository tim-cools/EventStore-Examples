using System;
using Soloco.EventStore.Core.Infrastructure;

namespace Soloco.EventStore.GamblingGameProjections.Projections
{
    public class IrresponsibleGamblingDetectorProjection
    {
        private readonly IProjectionContext _projectionsContext;

        public IrresponsibleGamblingDetectorProjection(IProjectionContext projectionsContext)
        {
            if (projectionsContext == null) throw new ArgumentNullException("projectionsContext");

            _projectionsContext = projectionsContext;
        }

        public void Ensure()
        {
            var projectionSource = ProjectionSources.Read("GameOverToPlayerDistributor");

            _projectionsContext.EnsureProjection("GameOverToPlayerDistributor", projectionSource);
        }
    }
}