namespace Soloco.EventStore.Test.MeasurementProjections.Queries
{
    public class MeasurementReadByDeviceTypePartitioner
    {
        public int DeviceType { get; set; }

        public override string ToString()
        {
            return string.Format("DeviceType: {0}", DeviceType);
        }
    }
}