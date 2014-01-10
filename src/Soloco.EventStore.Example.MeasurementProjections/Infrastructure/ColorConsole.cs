using System;

namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    public class ColorConsole : IColorConsole
    {
        private readonly object _lock = new object();

        public void Log(string value, params object[] arguments)
        {
            Log(value, arguments, ConsoleColor.Gray);
        }

        public void Green(string value, params object[] arguments)
        {
            Log(value, arguments, ConsoleColor.Green);
        }

        public void Yellow(string value, params object[] arguments)
        {
            Log(value, arguments, ConsoleColor.Yellow);
        }

        public void Magenta(string value, params object[] arguments)
        {
            Log(value, arguments, ConsoleColor.Magenta);
        }

        public void Cyan(string value, params object[] arguments)
        {
            Log(value, arguments, ConsoleColor.Cyan);
        }

        public void Red(string value, params object[] arguments)
        {
            Log(value, arguments, ConsoleColor.Red);
        }

        public void ReadLine()
        {
            Console.ReadLine();
        }

        private void Log(string message, object[] arguments, ConsoleColor color)
        {
            var value = arguments != null && arguments.Length > 0
                ? string.Format(message, arguments)
                : message;

            lock (_lock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(value);
            }
        }
    }
}