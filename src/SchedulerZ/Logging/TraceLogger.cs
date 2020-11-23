using SchedulerZ.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SchedulerZ
{
    public class TraceLogger : ILogger
    {
        private static readonly object _obj = new object();
        private static TraceLogger _instance;
        private List<ILogger> _loggers { get; set; }

        public static TraceLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_obj)
                    {
                        if (_instance == null)
                            _instance = new TraceLogger();
                    }
                }
                return _instance;
            }
        }

        private TraceLogger()
        {
            _loggers = new List<ILogger>();

            //控制台日志
            var consoleLogger = new ConsoleLogger();
            _loggers.Add(consoleLogger);

            //文件日志
            var textFileLogger = TextFileLogger.Create();
            _loggers.Add(textFileLogger);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            var log = GetLogger<TextFileLogger>();
            log?.Dispose();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex) Write(LogLevel.Fatal, ex, "Current Domain Unhandled Exception");
            if (e.IsTerminating)
            {
                Write(LogLevel.Fatal, null, "Abnormal Exit");

                var log = GetLogger<TextFileLogger>();
                log?.Dispose();
            }
        }

        public TLogger GetLogger<TLogger>() where TLogger : class, ILogger
        {
            foreach (var item in _loggers)
            {
                if (item != null)
                {
                    if (item is TLogger) return item as TLogger;
                }
            }
            return null;
        }

        public void AddLogger(ILogger logger)
        {
            if (_loggers != null)
                _loggers.Add(logger);
        }

        public void Write(LogLevel level, Exception ex, string format, params object[] args)
        {
            if (_loggers != null)
            {
                foreach (var logger in _loggers)
                {
                    logger.Write(level, ex, string.Format(format, args));
                }
            }
        }

        public void Trace(string message) => Write(LogLevel.Trace, null, message);
        public void Trace(string format, params object[] args) => Write(LogLevel.Trace, null, format, args);
        public void Trace(string message, Exception ex) => Write(LogLevel.Trace, ex, message);

        public void Debug(string message) => Write(LogLevel.Debug, null, message);
        public void Debug(string format, params object[] args) => Write(LogLevel.Debug, null, format, args);
        public void Debug(string message, Exception ex) => Write(LogLevel.Debug, ex, message);

        public void Info(string message) => Write(LogLevel.Info, null, message);
        public void Info(string format, params object[] args) => Write(LogLevel.Info, null, format, args);
        public void Info(string message, Exception ex) => Write(LogLevel.Info, ex, message);

        public void Error(string message) => Write(LogLevel.Error, null, message);
        public void Error(string format, params object[] args) => Write(LogLevel.Error, null, format, args);
        public void Error(string message, Exception ex) => Write(LogLevel.Error, ex, message);

        public void Fatal(string message) => Write(LogLevel.Fatal, null, message);
        public void Fatal(string format, params object[] args) => Write(LogLevel.Fatal, null, format, args);
        public void Fatal(string message, Exception ex) => Write(LogLevel.Fatal, ex, message);
    }
}
