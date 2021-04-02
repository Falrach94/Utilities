using ServerGUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogUtils.UI
{
    public class LogBox : RichTextBox
    {
        public readonly List<string> InstanceFilter = new List<string>();
        private readonly Dictionary<string, CategoryConfiguration> _categoryDic = new Dictionary<string, CategoryConfiguration>();
        private readonly List<LogMessage> _messages = new List<LogMessage>();
        private DateTime _lastVisibleLogDate;

        public int MinimalSkipTime { get; set; } = 100;

        public LogBox()
        {
            this.DoubleClick += Log_DoubleClick;
            this.BackColor = System.Drawing.SystemColors.MenuText;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.Info;

        }

        private void Log_DoubleClick(object sender, EventArgs e)
        {
            var selector = new LogSelector();
            selector.SetCategories(_categoryDic);
            selector.ShowDialog();
            Reload();
        }
        public void AddCategory(string name, bool show)
        {
            AddCategory(name, LogLevel.Debug, show, default);
        }

        public void AddCategory(string name, LogLevel minLevel, bool show, Color color)
        {
            _categoryDic.Add(name, new CategoryConfiguration(show, minLevel, name, color));
        }
        private bool IsVisible(LogMessage msg)
        {
            if(InstanceFilter.Count != 0 && !InstanceFilter.Contains(msg.Instance))
            {
                return false;
            }

            return _categoryDic[msg.LogModule].Show && _categoryDic[msg.LogModule].MinLevel <= msg.Level;
        }
        private void MaybeAddTimeSkip(DateTime date)
        {
            if (_lastVisibleLogDate != default && (date - _lastVisibleLogDate).TotalMilliseconds > MinimalSkipTime)
            {
                AppendText("----------------------------------------------------\n");
            }
            _lastVisibleLogDate = date;
        }

        public void Reload()
        {
            _lastVisibleLogDate = default;
            Clear();

            foreach (LogMessage m in _messages)
            {
                if (IsVisible(m))
                {
                    MaybeAddTimeSkip(m.Date);

                    this.AppendLogMessage(m, _categoryDic[m.LogModule].Color);
                }
            }
        }


        public void LogMessageHandler(object _, LogMessage logMessage)
        {
            string module = logMessage.LogModule;
            _messages.Add(logMessage);
            if (!_categoryDic.ContainsKey(module))
            {
                var config = new CategoryConfiguration(true, LogLevel.Debug, module);
                try
                {
                    _categoryDic.Add(module, config);
                }
                catch { }
            }
            try
            {
                Invoke((MethodInvoker)delegate
                {
                    var cat = _categoryDic[logMessage.LogModule];
                    if (IsVisible(logMessage))
                    {
                        MaybeAddTimeSkip(logMessage.Date);
                        this.AppendLogMessage(logMessage, cat.Color);
                    }
                });
            }
            catch { }
        }
    }
}
