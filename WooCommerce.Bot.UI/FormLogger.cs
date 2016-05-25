using System;
using System.Drawing;
using System.Windows.Forms;
using WooCommerce.Automation;

namespace WooCommerce.Bot.UI
{
    public class FormLogger : ILogger
    {
        private readonly Form form;
        private readonly RichTextBox richTextBox;
        public FormLogger(Form form, RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
            this.form = form;
        }

        public void Info(string message)
        {
            RunOnUIThread(() =>
            {
                AppendText(this.richTextBox, message + "\r\n", Color.DarkBlue);
            });
        }

        public void Warn(string message)
        {
            RunOnUIThread(() =>
            {
                AppendText(this.richTextBox, message + "\r\n", Color.DarkGoldenrod);
            });
        }

        public void Error(string message, Exception ex)
        {
            RunOnUIThread(() =>
            {
                AppendText(this.richTextBox, message + " " + ex.ToString() + "\r\n", Color.DarkRed);
            });
        }

        private void RunOnUIThread(Action action)
        {
            this.form.Invoke((MethodInvoker)delegate
            {
                action();
            });
        }

        public static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}