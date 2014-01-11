using System;

namespace Soloco.EventStore.MeasurementProjections.Events
{
    public class MeasurementRead
    {
        public DateTime Timestamp { get; set; }
        public decimal Reading { get; set; }

        public MeasurementRead()
        {
        }

        public MeasurementRead(DateTime timestamp, decimal value)
        {
            Timestamp = timestamp;
            Reading = value;
        }

        public override string ToString()
        {
            return string.Format("Timestamp:  {0}.{1}  Reading: {2}", Timestamp, Timestamp.Millisecond, Reading);
        }
    }
}