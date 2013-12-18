using System;

namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    public class MeasurementConsole : IMeasurementConsole
    {
        private readonly object _lock = new object();

        public void Log(string value)
        {
            Log(value, ConsoleColor.Gray);
        }

        public void Green(string value)
        {
            Log(value, ConsoleColor.Green);
        }

        public void Yellow(string value)
        {
            Log(value, ConsoleColor.Yellow);
        }

        public void Magenta(string value)
        {
            Log(value, ConsoleColor.Magenta);
        }

        public void Cyan(string value)
        {
            Log(value, ConsoleColor.Cyan);
        }

        public void Red(string value)
        {
            Log(value, ConsoleColor.Red);
        }

        private void Log(string value, ConsoleColor color)
        {
            lock (_lock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(value);
            }
        }
    }
}