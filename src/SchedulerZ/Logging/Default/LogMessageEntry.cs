using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Logging
{
    public readonly struct LogMessageEntry
    {
        public LogMessageEntry(string message, string timeStamp = null, string levelString = null, ConsoleColor? levelBackground = null, ConsoleColor? levelForeground = null, ConsoleColor? messageColor = null)
        {
            TimeStamp = timeStamp;
            LevelString = levelString;
            LevelBackground = levelBackground;
            LevelForeground = levelForeground;
            MessageColor = messageColor;
            Message = message;
        }

        public readonly string TimeStamp;
        public readonly string LevelString;
        public readonly ConsoleColor? LevelBackground;
        public readonly ConsoleColor? LevelForeground;
        public readonly ConsoleColor? MessageColor;
        public readonly string Message;
    }
}
