using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SchedulerZ.Logging
{
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5
    }

    public class DefaultLogger : ILogger, IDisposable
    {
        private readonly ConsoleColor? DefaultConsoleColor = null;

        public DefaultLogger()
        {

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
            var colors = GetLogLevelConsoleColors(logLevel);
            var colorChanged = SetColor(colors.Background, colors.Foreground);
            Console.Out.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [{Thread.CurrentThread.ManagedThreadId}] {logLevel.ToString().ToUpper()}  {message}");
            if (ex != null)
                Console.Out.WriteLine(ex.ToString());

            if (colorChanged)
            {
                Console.ResetColor();
            }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }
    }


    public class DefaultLoggerEntity
    {
        public DefaultLoggerEntity(LogLevel logLevel, string message, Exception ex)
        {
            LogLevel = logLevel;
            Message = message;
            Ex = ex;
        }

        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public Exception Ex { get; set; }
    }
}
