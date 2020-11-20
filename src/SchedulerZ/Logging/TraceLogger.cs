using SchedulerZ.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SchedulerZ
{
    public class TraceLogger: ILogger
    {
        public List<ILogger> _loggers { get; set; } = new List<ILogger>();

        private static ILogger _Log;

        public static ILogger Log { get { InitLog(); return _Log; } set { _Log = value; } }

        static readonly object _lock = new object();
        static int _initing = 0;

        /// <summary>文本日志目录</summary>
        public static string LogPath { get; set; } = "logs";

        static bool Init()
        {
            if (_Log != null && _Log != Logger.Null) return true;
            if (_initing > 0 && _initing == Thread.CurrentThread.ManagedThreadId) return false;

            lock (_lock)
            {
                if (_Log != null && _Log != Logger.Null) return true;

                _initing = Thread.CurrentThread.ManagedThreadId;

                _Log = TextFileLogger.Create(LogPath);


                if (!set.NetworkLog.IsNullOrEmpty())
                {
                    var nlog = new NetworkLog(set.NetworkLog);
                    _Log = new CompositeLog(_Log, nlog);
                }

                _initing = 0;
            }

            return true;
        }

        void Write(LogLevel level, Exception ex, string format, params object[] args)
        {
            if (_loggers != null)
            {
                foreach (var logger in _loggers)
                {
                    logger(level, message, ex);
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
