using System;
using Ninject;

namespace Soloco.EventStore.ECommerce.Infrastructure
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IKernel _kernel;

        public QueryDispatcher(IKernel kernel)
        {
            if (kernel == null) throw new ArgumentNullException("kernel");

            _kernel = kernel;
        }

        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var handler = GetQueryHandler(query);

            return handler.Handle((dynamic)query);
        }

        private dynamic GetQueryHandler<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IHandleQuery<,>).MakeGenericType(query.GetType(), typeof(TResult));

            return _kernel.Get(handlerType);
        }
    }
}