namespace Soloco.EventStore.MeasurementProjections.Queries
{
    public class MeasurementReadCounter
    {
        public int Count { get; set; }

        public override string ToString()
        {
            return string.Format("Count: {0}", Count);
        }
    }
}