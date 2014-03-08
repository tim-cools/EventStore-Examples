using System.Collections.Generic;
using Soloco.EventStore.ECommerce.Infrastructure;
using Soloco.EventStore.ECommerce.Model.ProductsView;

namespace Soloco.EventStore.ECommerce.Model.Products
{
    public class ProductsQuery : IQuery<IEnumerable<Product>>
    {
    }
}