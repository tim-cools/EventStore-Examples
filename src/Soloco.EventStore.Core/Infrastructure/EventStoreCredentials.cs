using EventStore.ClientAPI.SystemData;

namespace Soloco.EventStore.Core.Infrastructure
{
    public class EventStoreCredentials
    {
        private static readonly UserCredentials _credentials = new UserCredentials("admin", "changeit");

        public static UserCredentials Default { get { return _credentials; } }
    }
}