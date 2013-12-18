
namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    internal interface IMeasurementConsole
    {
        void Log(string value);
        void Green(string value);
        void Yellow(string value);
        void Magenta(string value);
        void Cyan(string value);
    }
}