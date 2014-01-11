namespace Soloco.EventStore.MeasurementProjections.Events
{
    public class MeasurementPeriod
    {
        public MeasurementPeriodType Type
        {
            get { return MeasurementPeriodType.FromDigit(Timeslot); }
        }
        
        public string Timeslot { get; set; }
        public decimal Total { get; set; }
        public decimal Count { get; set; }
        public decimal Average { get; set; }

        public override string ToString()
        {
            return string.Format("Type: {0}, Timeslot: {1}, Total: {2}, Count: {3}, Average: {4}", Type, Timeslot, Total, Count, Average);
        }
    }
}