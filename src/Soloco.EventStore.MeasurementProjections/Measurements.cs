using System.Reflection;

namespace Soloco.EventStore.MeasurementProjections
{
    public static class Measurements
    {
        public static Assembly Assembly
        {
            get
            {
                return typeof(Measurements).Assembly;
            }
        }
    }
}
