using System;
using System.Collections.Generic;
using System.Linq;

namespace Soloco.EventStore.Test.MeasurementProjections.Events
{
    public class MeasurementPeriodType
    {
        private static readonly IList<MeasurementPeriodType> _types = new List<MeasurementPeriodType>();

        private readonly char _type;

        public static MeasurementPeriodType Hour { get; private set; }
        public static MeasurementPeriodType Days { get; private set; }
        public static MeasurementPeriodType Month { get; private set; }

        private MeasurementPeriodType(char type)
        {
            _type = type;
        }

        static MeasurementPeriodType()
        {
            Hour = Add(new MeasurementPeriodType('H'));
            Days = Add(new MeasurementPeriodType('D'));
            Month = Add(new MeasurementPeriodType('M'));
        }

        private static MeasurementPeriodType Add(MeasurementPeriodType type)
        {
            _types.Add(type);
            return type;
        }

        protected bool Equals(MeasurementPeriodType other)
        {
            return string.Equals(_type, other._type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MeasurementPeriodType)obj);
        }

        public override int GetHashCode()
        {
            return _type.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Type: {0}", _type);
        }

        public static MeasurementPeriodType FromDigit(string timeslot)
        {
            if (string.IsNullOrEmpty(timeslot)) throw new InvalidOperationException("Unknown MeasurementPeriodType");

            var digit = timeslot[0];

            var type = _types.FirstOrDefault(t => t._type == digit);
            if (type == null)
            {
                throw new InvalidOperationException("Unknown MeasurementPeriodType");
            }
            return type;
        }
    }
}