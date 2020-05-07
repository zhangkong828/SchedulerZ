using log4net;
using System;

namespace SchedulerZ.Logging.Log4Net
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _log;

        public Log4NetLogger(ILog log)
        {
            _log = log;
        }

        public void Debug(string message)
        {
            _log.Debug(message);
        }

        public void Debug(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        public void Debug(string message, Exception ex)
        {
            _log.Debug(message, ex);
        }

        public void Error(string message)
        {
            _log.Error(message);
        }

        public void Error(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }

        public void Error(string message, Exception ex)
        {
            _log.Error(message, ex);
        }

        public void Fatal(string message)
        {
            _log.Fatal(message);
        }

        public void Fatal(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }

        public void Fatal(string message, Exception ex)
        {
            _log.Fatal(message, ex);
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Info(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        public void Info(string message, Exception ex)
        {
            _log.Info(message, ex);
        }

        public void Trace(string message)
        {
            _log.Debug(message);
        }

        public void Trace(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        public void Trace(string message, Exception ex)
        {
            _log.Debug(message, ex);
        }

        public void Warning(string message)
        {
            _log.Warn(message);
        }

        public void Warning(string format, params object[] args)
        {
            _log.WarnFormat(format, args);
        }

        public void Warning(string message, Exception ex)
        {
            _log.Warn(message, ex);
        }
    }
}
