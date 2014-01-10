
namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    public interface IColorConsole
    {
        void Log(string message, params object[] arguments);
        void Green(string message, params object[] arguments);
        void Yellow(string message, params object[] arguments);
        void Magenta(string message, params object[] arguments);
        void Cyan(string message, params object[] arguments);
        void Red(string message, params object[] arguments);

        void ReadLine();
    }
}