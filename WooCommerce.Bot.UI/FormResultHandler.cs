using System;
using System.Windows.Forms;
using WooCommerce.Automation;
using WooCommerce.Automation.Models;

namespace WooCommerce.Bot.UI
{
    public class FormResultHandler : IResultHandler
    {
        private readonly Form form;
        private int index = 0;
        private ListView listView1;

        public FormResultHandler(Form form, ListView listView)
        {
            this.listView1 = listView;
            this.form = form;
        }

        public void Handle(AliExpressPostResult result)
        {
            var listViewSubItem1 = new ListViewItem.ListViewSubItem();
            listViewSubItem1.Text = result.SourceUrl;

            var listViewSubItem2 = new ListViewItem.ListViewSubItem();
            listViewSubItem2.Text = result.Success ? "Success" : "Failed";

            var listViewSubItem3 = new ListViewItem.ListViewSubItem();
            listViewSubItem3.Text = result?.Name;

            var listViewSubItem4 = new ListViewItem.ListViewSubItem();
            listViewSubItem4.Text = result?.PostedUrl;

            var listViewSubItem5 = new ListViewItem.ListViewSubItem();
            listViewSubItem5.Text = result?.Reason;

            var listViewItem = new ListViewItem(index++.ToString());
            listViewItem.SubItems.Add(listViewSubItem1);
            listViewItem.SubItems.Add(listViewSubItem2);
            listViewItem.SubItems.Add(listViewSubItem3);
            listViewItem.SubItems.Add(listViewSubItem4);
            listViewItem.SubItems.Add(listViewSubItem5);

            RunOnUIThread(() =>
            {
                listView1.Items.Add(listViewItem);
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