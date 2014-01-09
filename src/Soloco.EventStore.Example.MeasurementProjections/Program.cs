using System;
using System.Net;

using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using Soloco.EventStore.Test.MeasurementProjections.Infrastructure;

namespace Soloco.EventStore.Test.MeasurementProjections
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = CreateConnection();
            var logger = new ConsoleLogger();
            var projections = new ProjectionsManager(logger, CreateTcpEndPoint(2113));

            var console = new MeasurementConsole();

            var projectionContext = new ProjectionContext(projections, console);
            projectionContext.Initialize();

            var reader = new ProjectionReader(connection, console);
            reader.Start();

            var simulator = new MeasurementReadSimulator(connection, console);
            simulator.Start();

            var queryReader = new QueryReader(connection);
            queryReader.Start();

            Console.ReadLine();

            simulator.Stop();

            Console.ReadLine();
        }

        public static IEventStoreConnection CreateConnection()
        {
            var tcpEndPoint = CreateTcpEndPoint(1113);

            var connection = EventStoreConnection.Create(tcpEndPoint);
            connection.Connect();
            return connection;
        }

        private static IPEndPoint CreateTcpEndPoint(int port)
        {
            var address = IPAddress.Parse("127.0.0.1");
            return new IPEndPoint(address, port);
        }
    }
}
