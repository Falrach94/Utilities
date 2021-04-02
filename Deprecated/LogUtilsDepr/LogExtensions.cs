using NLog;
using System.Diagnostics;

namespace GameServer
{
    static public class LogExtensions
    {
        public static string GetCallingMethodName()
        {
            StackTrace trace = new StackTrace();
            return trace.GetFrame(2).GetMethod().Name;
        }

        public static void TraceEntry(this Logger logger)
        {
            logger.Trace(GetCallingMethodName() + " entry");
        }
        public static void TraceExit(this Logger logger)
        {
            logger.Trace(GetCallingMethodName() + " exit");
        }
    }
}
