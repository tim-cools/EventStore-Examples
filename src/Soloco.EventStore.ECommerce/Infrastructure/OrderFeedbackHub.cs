using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.ECommerce.Model.Order;
using Soloco.EventStore.ECommerce.Model.Products;
using Soloco.EventStore.ECommerce.Model.ProductsView;

namespace Soloco.EventStore.ECommerce.Infrastructure
{
    [HubName("OrderCreator")]
    public class OrderCreatorHub : Hub
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IConnectionManager _connectionManager;

        public OrderCreatorHub(ICommandDispatcher commandDispatcher, ICommandResponseWatcher commandResponseWatcher, IQueryDispatcher queryDispatcher, IConnectionManager connectionManager)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _connectionManager = connectionManager;

            commandResponseWatcher.ResponseReceived += CommandResponseReceived;
        }

        public void RegisterClient(string userId)
        {
            var connectionId = Context.ConnectionId;
            var hub = _connectionManager.GetHubContext<OrderCreatorHub>();

            Task.WaitAll(hub.Groups.Add(connectionId, userId));
        }
        
        public IEnumerable<Product> ReceiveProducts()
        {
            return _queryDispatcher.Execute(new ProductsQuery());         
        }

        public void CreateOrder(string userId)
        {
            var orderId = IdGenerator.New();
            _commandDispatcher.Execute("Order", orderId, new CreateOrder(orderId, userId));
        }

        private void CommandResponseReceived(object sender, CommandResponseEventArgs e)
        {
            var clients = GetClientsToInform(e.Response.UserId);
            clients.CommandResponseReceived(e.Response);
        }

        private dynamic GetClientsToInform(string userId)
        {
            var hub = _connectionManager.GetHubContext<OrderCreatorHub>();            
            return hub.Clients.Group(userId);
        }
    }
}