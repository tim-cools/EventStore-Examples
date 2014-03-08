using Soloco.EventStore.Core;
using Soloco.EventStore.Core.Infrastructure;
using Soloco.EventStore.ECommerce.Infrastructure;

namespace Soloco.EventStore.ECommerce.Server
{
    internal class Server
    {
        private readonly IProjectionContext _projectionContext;

        private readonly Projections.OrderHandler _orderHandler;
        
        private readonly WebServer _webServer;

        private readonly EventReader _eventReader;

        private readonly IConsole _console;

        public Server(IProjectionContext projectionContext, EventReader eventReader, IConsole console, Projections.OrderHandler orderHandler, WebServer webServer)
        {
            _projectionContext = projectionContext;
            _eventReader = eventReader;
            _console = console;
            _orderHandler = orderHandler;
            _webServer = webServer;
        }

        static void Main()
        {
            var server = KernelFactory.Get<Server>(new KernelModule(), ECommerce.Assembly);

            server.Run();
        }
        
        public void Run()
        {
            EnsureProjections();

            Start();

            _console.ReadKey("Press any key to stop sample...");

            Stop();
        }

        private void Start()
        {
            _eventReader.StartReading();

            _webServer.Start();
        }

        private void EnsureProjections()
        {
            _projectionContext.EnableProjection("$by_category");
            _projectionContext.EnableProjection("$stream_by_category");

            _orderHandler.Ensure();
        }

        private void Stop()
        {
            _webServer.Stop();
        }
    }
}