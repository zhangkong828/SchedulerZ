using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Logging
{
    public interface ILoggerProvider
    {
        ILogger CreateLogger(string name);

        ILogger CreateLogger<T>();
    }
}
