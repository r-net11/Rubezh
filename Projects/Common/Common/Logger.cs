using System;
using System.IO;
using NLog;

namespace Common
{
    public static class Logger
    {
        private static NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        private static object[] _empty = new object[0];

        static Logger()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        public static void Error(Exception ex)
        {
            Error(ex, ex.Message, _empty);
        }
        public static void Error(Exception ex, string message)
        {
            Error(ex, message, _empty);
        }
        public static void Error(Exception ex, string message, params object[] args)
        {
            _logger.LogException(LogLevel.Error, string.Format(message, args), ex);
        }
        public static void Error(string message)
        {
            Error(message, _empty);
        }
        public static void Error(string message, params object[] args)
        {
            WriteLog(LogLevel.Error, message, args);
        }
        public static void Warn(string message)
        {
            Warn(message, _empty);
        }
        public static void Warn(string message, params object[] args)
        {
            WriteLog(LogLevel.Warn, message, args);
        }
        public static void Info(string message)
        {
            Info(message, _empty);
        }
        public static void Info(string message, params object[] args)
        {
            WriteLog(LogLevel.Info, message, args);
        }
        public static void Trace(string message)
        {
            Trace(message, _empty);
        }
        public static void Trace(string message, params object[] args)
        {
            WriteLog(LogLevel.Trace, message, args);
        }

        private static void WriteLog(LogLevel level, string message, params object[] args)
        {
            _logger.Log(level, message, args);
        }
    }
}