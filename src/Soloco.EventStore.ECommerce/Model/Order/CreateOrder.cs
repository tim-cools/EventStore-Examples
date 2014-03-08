using System;
using System.Collections.Generic;
using Soloco.EventStore.ECommerce.Infrastructure;

namespace Soloco.EventStore.ECommerce.Model.Order
{
    //: fun
    //AddOrderLine: fu
    //RemoveOrderLine:
    //SubmitOrder: fun
    
    //OrderCreated: fu
    //OrderLineAdded: 
    //OrderLineRemoved
    //OrderSubmit: fun

    public class CreateOrder : ICommand
    {
        public string OrderId { get; private set; }
        public string UserId { get; private set; }

        public CreateOrder(string orderId, string userId)
        {
            OrderId = orderId;
            UserId = userId;
        }
    }
}