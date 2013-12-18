using System;
using System.Text;

using EventStore.ClientAPI;

using Newtonsoft.Json;

namespace Soloco.EventStore.Test.MeasurementProjections.Events
{
    public class JsonEvent
    {
        //public Guid EventId { get; private set; }
        //public string Type { get; private set; }
        //public bool IsJson { get { return true; } }

        //public byte[] Data { get; private set; }
        //public byte[] Metadata { get; private set; }

        //private JsonEvent(string type, byte[] data)
        //{
        //    if (data == null) throw new ArgumentNullException("data");

        //    EventId = Guid.NewGuid();
        //    Type = type;
        //    Data = data;
        //    Metadata = new byte[] { };
        //}

        //public override string ToString()
        //{
        //    return string.Format("{0}: {1}", Type, Encoding.UTF8.GetString(Data));
        //}

        public static EventData Create<T>(T value)
        {
            var json = JsonConvert.SerializeObject(value);
            var data = Encoding.UTF8.GetBytes(json);
            var eventName = typeof(T).Name;

            return new EventData(Guid.NewGuid(), eventName, true, data, new byte[] {});
        }

        public static T Parse<T>(RecordedEvent data)
        {
            var value = Encoding.UTF8.GetString(data.Data);

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}