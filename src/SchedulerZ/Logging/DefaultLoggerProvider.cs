using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Logging
{
    public class DefaultLoggerProvider : ILoggerProvider
    {
        private static readonly DefaultLogger Logger = new DefaultLogger();

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
