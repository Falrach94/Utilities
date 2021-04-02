using NLog;
using NLog.Config;
using NLog.Targets;

namespace LogUtils
{
    [Target("Logger")]
    public sealed class CustomLogTarget : TargetWithLayout
    {
        public CustomLogTarget()
        {
            //set defaults
            this.Host = "localhost";
        }

        [RequiredParameter]
        public string Host { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            //   string logMessage = this.Layout.Render(logEvent);

            //TODO write to target
        }
    }
}
