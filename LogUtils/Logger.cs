using System;

namespace LogUtils
{
    public class Logger
    {
        public Action<LogMessage> NewLog { get; set; } = m => { };

        public string Name { get; }

        public Logger(string name)
        {
            Name = name;
        }

        public void Log(LogLevel level, string msg)
        {
            NewLog(new LogMessage()
            {
                Date = DateTime.Now,
                Level = level,
                Message = msg,
                LogModule = Name
            });
        }
        public void Trace(string msg)
        {
            Log(LogLevel.Trace, msg);
        }
        public void Debug(string msg)
        {
            Log(LogLevel.Debug, msg);
        }
        public void Info(string msg)
        {
            Log(LogLevel.Info, msg);
        }
        public void Warn(string msg)
        {
            Log(LogLevel.Warn, msg);
        }
        public void Error(string msg)
        {
            Log(LogLevel.Error, msg);
        }
        public void Error(Exception ex)
        {
            Log(LogLevel.Error, ex.ToString());
        }
        public void Error(string msg, Exception _)
        {
            Log(LogLevel.Error, msg);
        }
        public void Fatal(string msg)
        {
            Log(LogLevel.Fatal, msg);
        }

    }
}
