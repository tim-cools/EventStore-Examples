namespace Soloco.EventStore.MeasurementProjections.Events
{
    public class MeasurementAverageDay
    {
        public string Timeslot { get; set; }
        public decimal Total { get; set; }
        public decimal Count { get; set; }
        public decimal Average { get; set; }

        public override string ToString()
        {
            return string.Format("Timeslot: {0}, Total: {1}, Count: {2}, Average: {3}", Timeslot, Total, Count, Average);
        }
    }
}