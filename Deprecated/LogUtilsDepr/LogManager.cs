using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace LogUtils
{
    public class LogManager
    {
        #region fields
        private int _nextId = 0;

        private readonly Dictionary<string, Logger> _loggerDic = new Dictionary<string, Logger>();

        private readonly BinaryFormatter _bf = new BinaryFormatter();
        #endregion

        #region properties
        public string InstanceName { get; set; }
        public Stream Stream { get; set; }
        public static Logger DefaultLogger { get; } = new Logger("Default");
        #endregion

        public event EventHandler<LogMessage> NewLogMessageEvent;

        public LogManager(string name)
        {
            InstanceName = name;
        }

        public Logger GetLogger(string name)
        {
            if (!_loggerDic.ContainsKey(name))
            {
                var logger = new Logger(name)
                {
                    NewLog = NewLogMessage
                };

                _loggerDic.Add(name, logger);
            }
            return _loggerDic[name];
        }

        private void NewLogMessage(LogMessage msg)
        {
            NewLogMessageEvent?.Invoke(this, msg);
            if (Stream == null)
            {
                return;
            }
            lock (Stream)
            {
                msg.Id = _nextId++;
                msg.Instance = InstanceName;
                _bf.Serialize(Stream, msg);
                Monitor.PulseAll(Stream);
            }
        }
    }
}
