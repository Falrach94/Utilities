
using System;

namespace LogUtils
{
    [Serializable]
    public class LogMessage
    {

        public LogMessage()
        {
        }


        public string Instance { get; set; }
        public DateTime Date { get; set; }
        public string LogModule { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
    }
}
