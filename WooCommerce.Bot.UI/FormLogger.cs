using System;
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
                this.richTextBox.AppendText(message + "\r\n");
            });
        }

        public void Warn(string message)
        {
            RunOnUIThread(() =>
            {
                this.richTextBox.AppendText(message + "\r\n");
            });
        }

        public void Error(string message, Exception ex)
        {
            RunOnUIThread(() =>
            {
                this.richTextBox.AppendText(message + "\r\n");
            });
        }

        private void RunOnUIThread(Action action)
        {
            this.form.Invoke((MethodInvoker)delegate
            {
                action();
            });
        }
    }
}