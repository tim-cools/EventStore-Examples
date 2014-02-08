
using System;

namespace Soloco.EventStore.Core.Infrastructure
{
    public static class IdGenerator
    {
        public static string New()
        {
            return Guid.NewGuid()
                .ToString()
                .Replace("-", string.Empty);
        }
    }
}