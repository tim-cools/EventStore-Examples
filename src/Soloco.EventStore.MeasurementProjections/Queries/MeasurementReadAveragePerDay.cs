namespace Soloco.EventStore.MeasurementProjections.Queries
{
    public class MeasurementReadAveragePerDay
    {
        public string TimeSlot { get; private set; }
        public decimal Average { get; private set; }

        public MeasurementReadAveragePerDay(string timeSlot, decimal average)
        {
            TimeSlot = timeSlot;
            Average = average;
        }

        public override string ToString()
        {
            return string.Format("TimeSlot: {0}, Average: {1}", TimeSlot, Average);
        }
    }
}