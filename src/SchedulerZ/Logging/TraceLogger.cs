using SchedulerZ.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SchedulerZ
{
    public class TraceLogger : ILogger
    {
        private static readonly ConcurrentDictionary<string, TraceLogger> _loggers = new ConcurrentDictionary<string, TraceLogger>(StringComparer.OrdinalIgnoreCase);

        public static TraceLogger GetLogger()
        {
            return GetLogger(null);
        }

        public static TraceLogger GetLogger(string name)
        {
            var path = Config.LoggerOptions.FileLoggerPath;
            if (!string.IsNullOrWhiteSpace(name))
                path = path.CombinePath(name);

            return _loggers.GetOrAdd(path, key => new TraceLogger(path));
        }

        static TraceLogger()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            foreach (var logger in _loggers.Values)
            {
                logger.Release();
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                if (_loggers.TryGetValue(Config.LoggerOptions.FileLoggerPath, out TraceLogger logger))
                    logger.Write(LogLevel.Fatal, ex, "Current Domain Unhandled Exception");
            }

            if (e.IsTerminating)
            {
                foreach (var logger in _loggers.Values)
                {
                    logger.Release();
                }
            }
        }
        

        private readonly ConsoleLogger _consoleLogger;
        private readonly TextFileLogger _textFileLogger;
        private TraceLogger(string path)
        {
            _consoleLogger = new ConsoleLogger();
            _textFileLogger = new TextFileLogger(path, false);
        }

        public void Write(LogLevel level, Exception ex, string format, params object[] args)
        {
            if (Config.LoggerOptions.EnableConsoleLogger)
                _consoleLogger.Write(level, ex, string.Format(format, args));

            _textFileLogger.Write(level, ex, string.Format(format, args));
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

        public void Release()
        {
            _textFileLogger.Release();
        }
    }
}
