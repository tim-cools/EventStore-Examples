using System;
using Soloco.EventStore.Core.Infrastructure;

namespace Soloco.EventStore.ECommerce.Projections
{
    public class OrderHandler
    {
        private readonly IProjectionContext _projectionsContext;

        public OrderHandler(IProjectionContext projectionsContext)
        {
            if (projectionsContext == null) throw new ArgumentNullException("projectionsContext");

            _projectionsContext = projectionsContext;
        }

        public void Ensure()
        {
            var projectionSource = ProjectionSources.Read("OrderHandler");

            _projectionsContext.EnsureProjection("OrderHandler", projectionSource);
        }
    }
}