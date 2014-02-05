
using System.Reflection;

namespace Soloco.EventStore.GamblingGameProjections
{
    public static class GamblingGame
    {
        public static Assembly Assembly
        {
            get
            {
                return typeof(GamblingGame).Assembly;
            }
        }
    }
}
