using System;

namespace SchedulerZ.Logging
{
    public interface ILogger
    {
        void Trace(string message);
        void Trace(string format, params object[] args);
        void Trace(string message, Exception ex);

        void Debug(string message);
        void Debug(string format, params object[] args);
        void Debug(string message, Exception ex);

        void Info(string message);
        void Info(string format, params object[] args);
        void Info(string message, Exception ex);

        void Error(string message);
        void Error(string format, params object[] args);
        void Error(string message, Exception ex);

        void Fatal(string message);
        void Fatal(string format, params object[] args);
        void Fatal(string message, Exception ex);

        void Write(LogLevel level, Exception ex, string format, params object[] args);

        void Release();
    }
}
