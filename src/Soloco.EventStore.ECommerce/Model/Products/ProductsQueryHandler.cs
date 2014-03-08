using System.Collections.Generic;
using Soloco.EventStore.ECommerce.Infrastructure;
using Soloco.EventStore.ECommerce.Model.ProductsView;

namespace Soloco.EventStore.ECommerce.Model.Products
{
    public class ProductsQueryHandler : IHandleQuery<ProductsQuery, IEnumerable<Product>>
    {
        public IEnumerable<Product> Handle(ProductsQuery query)
        {
            return new[]
            {
                new Product {Id = "1", Name = "Book 1"},
                new Product {Id = "2", Name = "Book 2"},
                new Product {Id = "3", Name = "Book 3"},
                new Product {Id = "4", Name = "Book 4"},
                new Product {Id = "5", Name = "Book 5"},
                new Product {Id = "6", Name = "Book 6"},
            };
        }
    }
}