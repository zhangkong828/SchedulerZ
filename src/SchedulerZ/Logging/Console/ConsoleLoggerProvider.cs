using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Logging
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConsoleLogger Logger;

        public ConsoleLoggerProvider()
        {
            if (Runtime.Windows)
            {
                Logger = new ConsoleLogger(new WindowsLogConsole());
            }
            else
            {
                Logger = new ConsoleLogger(new AnsiLogConsole());
            }
        }


        public ILogger CreateLogger(string name)
        {
            return Logger;
        }

        public ILogger CreateLogger<T>()
        {
            return Logger;
        }
    }
}
