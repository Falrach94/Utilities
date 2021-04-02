using LogUtils;
using LogUtils.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ServerGUI
{
    public static class UILogMessage
    {
        public struct TextSegment
        {
            public TextSegment(Color textColor, Color backColor, string text)
            {
                BackColor = backColor;
                TextColor = textColor;
                Text = text;
            }
            public Color BackColor { get; }
            public Color TextColor { get; }
            public string Text { get; }

            public static TextSegment Create(string text)
            {
                return new TextSegment(default, default, text);
            }
            public static TextSegment Create(string text, Color textColor)
            {
                return new TextSegment(textColor, default, text);
            }
            public static TextSegment Create(string text, Color textColor, Color backColor)
            {
                return new TextSegment(textColor, backColor, text);
            }
        }

        private static readonly Color[] _levelColors = new[] { Color.Gray, Color.LightBlue, Color.White, Color.Yellow, Color.Red, Color.Gold };

        private static readonly List<TextSegment> _segments = new List<TextSegment>();


        public static void AppendLogMessage(this RichTextBox tb, LogMessage msg, Color color)
        {
            var levelColor = _levelColors[(int)msg.Level];
            _segments.Add(TextSegment.Create(msg.Instance + " "));
            _segments.Add(TextSegment.Create(msg.Date.TimeOfDay.ToString(), levelColor, default));
            _segments.Add(TextSegment.Create(" - "));
            _segments.Add(TextSegment.Create(msg.LogModule, color, default));
            _segments.Add(TextSegment.Create(": "));
            _segments.Add(TextSegment.Create(msg.Message, levelColor));

            AddToTextBox(tb);

            _segments.Clear();
        }

        private static string Text => String.Join("", _segments.Select(s => s.Text));

        private static void AddToTextBox(RichTextBox tb)
        {
            int i = tb.Text.Length;
            tb.AppendText(Text + "\n");
            foreach (var s in _segments)
            {
                tb.Select(i, s.Text.Length);
                i += s.Text.Length;
                if (s.TextColor != default)
                {
                    tb.SelectionColor = s.TextColor;
                }
                if (s.BackColor != default)
                {
                    tb.SelectionBackColor = s.BackColor;
                    tb.SelectionColor = Color.FromArgb(255 - s.BackColor.R, 255 - s.BackColor.G, 255 - s.BackColor.B);
                }

            }
            tb.Select(0, 0);

            tb.SelectionStart = tb.Text.Length;
            tb.ScrollToCaret();
        }
    }
}
