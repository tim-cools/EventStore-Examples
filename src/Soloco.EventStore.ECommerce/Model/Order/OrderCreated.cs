using Soloco.EventStore.ECommerce.Infrastructure;

namespace Soloco.EventStore.ECommerce.Model.Order
{
    public class OrderCreated : ICommand
    {
        public string OrderId { get; set; }

        public OrderCreated(string orderId)
        {
            OrderId = orderId;
        }
    }
}