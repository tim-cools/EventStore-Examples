using System.Net;

namespace Soloco.EventStore.Core.Infrastructure
{
    internal class IPEndPointFactory
    {
        public static IPEndPoint DefaultTcp()
        {
            return CreateTcpEndPoint(1113);
        }

        public static IPEndPoint DefaultHttp()
        {
            return CreateTcpEndPoint(2113);
        }

        private static IPEndPoint CreateTcpEndPoint(int port)
        {
            var address = IPAddress.Parse("127.0.0.1");
            return new IPEndPoint(address, port);
        }
    }
}