using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SchedulerZ.Logging
{
    public class ConsoleLogger : ILogger, IDisposable
    {
        private readonly ConsoleColor? DefaultConsoleColor = null;

        private readonly ConsoleLoggerProcessor _queueProcessor;

        private static readonly string _loglevelPadding = "  ";
        private static readonly string _messagePadding;
        private static readonly string _newLineWithMessagePadding;

        [ThreadStatic]
        private static StringBuilder _logBuilder;

        static ConsoleLogger()
        {
            var logLevelString = GetLogLevelString(LogLevel.Warning);
            _messagePadding = new string(' ', logLevelString.Length + _loglevelPadding.Length);
            _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
        }

        public ConsoleLogger(IConsole console)
        {
            _queueProcessor = new ConsoleLoggerProcessor(console);
        }

        public void Trace(string message)
        {
            WriteMessage(LogLevel.Trace, message, null);
        }

        public void Trace(string format, params object[] args)
        {
            WriteMessage(LogLevel.Trace, string.Format(format, args), null);
        }

        public void Trace(string message, Exception ex)
        {
            WriteMessage(LogLevel.Trace, message, ex);
        }

        public void Debug(string message)
        {
            WriteMessage(LogLevel.Debug, message, null);
        }

        public void Debug(string format, params object[] args)
        {
            WriteMessage(LogLevel.Debug, string.Format(format, args), null);
        }

        public void Debug(string message, Exception ex)
        {
            WriteMessage(LogLevel.Debug, message, ex);
        }

        public void Warning(string message)
        {
            WriteMessage(LogLevel.Warning, message, null);
        }

        public void Warning(string format, params object[] args)
        {
            WriteMessage(LogLevel.Warning, string.Format(format, args), null);
        }

        public void Warning(string message, Exception ex)
        {
            WriteMessage(LogLevel.Warning, message, ex);
        }

        public void Info(string message)
        {
            WriteMessage(LogLevel.Info, message, null);
        }

        public void Info(string format, params object[] args)
        {
            WriteMessage(LogLevel.Info, string.Format(format, args), null);
        }

        public void Info(string message, Exception ex)
        {
            WriteMessage(LogLevel.Info, message, ex);
        }

        public void Error(string message)
        {
            WriteMessage(LogLevel.Error, message, null);
        }

        public void Error(string format, params object[] args)
        {
            WriteMessage(LogLevel.Error, string.Format(format, args), null);
        }

        public void Error(string message, Exception ex)
        {
            WriteMessage(LogLevel.Error, message, ex);
        }

        public void Fatal(string message)
        {
            WriteMessage(LogLevel.Fatal, message, null);
        }

        public void Fatal(string format, params object[] args)
        {
            WriteMessage(LogLevel.Fatal, string.Format(format, args), null);
        }

        public void Fatal(string message, Exception ex)
        {
            WriteMessage(LogLevel.Fatal, message, ex);
        }


        private void WriteMessage(LogLevel logLevel, string message, Exception ex)
        {
            var logBuilder = _logBuilder;
            _logBuilder = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }

            var logLevelColors = GetLogLevelConsoleColors(logLevel);
            var logLevelString = GetLogLevelString(logLevel);

            logBuilder.Append(_loglevelPadding);

            if (!string.IsNullOrEmpty(message))
            {
                // message
                logBuilder.Append(_loglevelPadding);

                var len = logBuilder.Length;
                logBuilder.AppendLine(message);
                logBuilder.Replace(Environment.NewLine, _newLineWithMessagePadding, len, message.Length);
            }

            if (ex != null)
            {
                logBuilder.AppendLine(ex.ToString());
            }

            var messagePadding= new string(' ', _messagePadding.Length - logLevelString.Length);
            var logMessage = new LogMessageEntry(
                message: logBuilder.ToString(),
                timeStamp: string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), messagePadding),
                levelString: logLevelString,
                levelBackground: logLevelColors.Background,
                levelForeground: logLevelColors.Foreground,
                messageColor: DefaultConsoleColor
                );
            _queueProcessor.EnqueueMessage(logMessage);

            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }
            _logBuilder = logBuilder;
        }

        private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Fatal:
                    return new ConsoleColors(ConsoleColor.White, ConsoleColor.Red);
                case LogLevel.Error:
                    return new ConsoleColors(ConsoleColor.Black, ConsoleColor.Red);
                case LogLevel.Warning:
                    return new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black);
                case LogLevel.Info:
                    return new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black);
                case LogLevel.Debug:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
                case LogLevel.Trace:
                    return new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
                default:
                    return new ConsoleColors(DefaultConsoleColor, DefaultConsoleColor);
            }
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trace";
                case LogLevel.Debug:
                    return "debug";
                case LogLevel.Info:
                    return "info";
                case LogLevel.Warning:
                    return "warning";
                case LogLevel.Error:
                    return "error";
                case LogLevel.Fatal:
                    return "fatal";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private bool SetColor(ConsoleColor? background, ConsoleColor? foreground)
        {
            if (background.HasValue)
            {
                Console.BackgroundColor = background.Value;
            }

            if (foreground.HasValue)
            {
                Console.ForegroundColor = foreground.Value;
            }

            return background.HasValue || foreground.HasValue;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _queueProcessor.Dispose();
            }
        }

        private readonly struct ConsoleColors
        {
            public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
            {
                Foreground = foreground;
                Background = background;
            }

            public ConsoleColor? Foreground { get; }

            public ConsoleColor? Background { get; }
        }

    }
}
