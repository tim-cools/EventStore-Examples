using System;
using System.Text;

using EventStore.ClientAPI;

using Newtonsoft.Json;

namespace Soloco.EventStore.Test.MeasurementProjections.Events
{
    public static class JsonEventExtensions
    {
        public static EventData AsJson(this object value)
        {
            if (value == null) throw new ArgumentNullException("value");

            var json = JsonConvert.SerializeObject(value);
            var data = Encoding.UTF8.GetBytes(json);
            var eventName = value.GetType().Name;

            return new EventData(Guid.NewGuid(), eventName, true, data, new byte[] {});
        }

        public static T ParseJson<T>(this RecordedEvent data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var value = Encoding.UTF8.GetString(data.Data);

            return JsonConvert.DeserializeObject<T>(value);
        }

        public static T ParseJson<T>(this ResolvedEvent data)
        {
            var value = Encoding.UTF8.GetString(data.Event.Data);
            
            //Console.WriteLine("RAW Event ({0}):{1}", data.OriginalEvent.EventType, value);

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}