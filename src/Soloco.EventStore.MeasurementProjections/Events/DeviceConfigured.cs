
namespace Soloco.EventStore.MeasurementProjections.Events
{
    public class DeviceConfigured
    {
        public string DeviceType { get; set; }
        
        public string Name { get; set; }

        public DeviceConfigured()
        {
        }

        public DeviceConfigured(string deviceType, string name)
        {
            DeviceType = deviceType;
            Name = name;
        }
    }
}