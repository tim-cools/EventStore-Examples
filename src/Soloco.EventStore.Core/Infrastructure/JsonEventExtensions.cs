using System;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Soloco.EventStore.Core.Infrastructure
{
    public static class JsonEventExtensions
    {
        public static string AsJsonString(this object value)
        {
            if (value == null) throw new ArgumentNullException("value");

            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public static EventData AsJsonEvent(this string value, string eventName)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            return new EventData(Guid.NewGuid(), eventName, true, bytes, null);
        }

        public static EventData AsJsonEvent(this object value)
        {
            if (value == null) throw new ArgumentNullException("value");

            var json = JsonConvert.SerializeObject(value);
            var eventName = value.GetType().Name;
            
            return json.AsJsonEvent(eventName);
        }

        public static T ParseJson<T>(this RecordedEvent data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var value = Encoding.UTF8.GetString(data.Data);

            return JsonConvert.DeserializeObject<T>(value);
        }

        public static T ParseJson<T>(this string value)
        {
            if (value == null) throw new ArgumentNullException("value");

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