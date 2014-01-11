using System;

namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    public class Console : IConsole
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

        public void Timings(string value, params object[] arguments)
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

        public void Error(string value, params object[] arguments)
        {
            Log(value, arguments, ConsoleColor.Red);
        }

        public void Important(string value, params object[] arguments)
        {
            Log(value, arguments, ConsoleColor.White);
        }

        public void ReadLine()
        {
            System.Console.ReadLine();
        }

        private void Log(string message, object[] arguments, ConsoleColor color)
        {
            var value = arguments != null && arguments.Length > 0
                ? string.Format(message, arguments)
                : message;

            lock (_lock)
            {
                System.Console.ForegroundColor = color;
                System.Console.WriteLine(value);
            }
        }
    }
}