using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WooCommerce.Automation;
using WooCommerceNET;
using WooCommerceNET.WooCommerce;

namespace WooCommerce.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private bool started = false;

        private void button1_Click(object sender, EventArgs e)
        {
            //RestAPI rest = new RestAPI("http://dealliaomah.com/wc-api/v3/",
            //       "ck_3586593efc9d88b09d62356e76924199b12d1221",
            //       "cs_ec7a0eb857e95790f7bcd1828b37d9fe7542f154");
            //WCObject wc = new WCObject(rest);



            //new Thread(() =>
            //{
            //    var category1 = GetCategory(wc, "wilson1", "");
            //    var category2 = GetCategory(wc, "wilson2", "wilson1");
            //    var category3 = GetCategory(wc, "wilson3", "wilson2");

            //    Console.WriteLine(category3);
            //}).Start();
            //return;

            if (!started)
            {
                var urls = richTextBox1.Lines.ToList();
                richTextBox1.Enabled = false;
                button1.Text = "Stop";
                started = true;

                new Thread(() =>
                {
                    foreach (var text in urls)
                    {
                        if (!started)
                            break;
                        try
                        {
                            var poster = new AliExpressPoster();
                            var result = poster.Generate(text);

                            RunOnUIThread(() =>
                            {
                                CreateListViewItem(result);
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        Thread.Sleep(5000);
                    }
 
                    RunOnUIThread(() =>
                    {
                        HandleStop();
                    });
                }).Start();


            }
            else
            {
                started = false;
            }
        }

        private void HandleStop()
        {
            richTextBox1.Enabled = true;
            button1.Text = "Start";
            started = false;

            MessageBox.Show("Done!");
        }

        private void RunOnUIThread(Action action)
        {
            this.Invoke((MethodInvoker)delegate
            {
                action();
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private int index = 0;
        private void CreateListViewItem(AliExpressPostResult result)
        {
            var listViewSubItem1 = new ListViewItem.ListViewSubItem();
            listViewSubItem1.Text = result.SourceUrl;

            var listViewSubItem2 = new ListViewItem.ListViewSubItem();
            listViewSubItem2.Text = result.Success ? "Success" : "Failed";

            var listViewSubItem3 = new ListViewItem.ListViewSubItem();
            listViewSubItem3.Text = result?.Name;

            var listViewSubItem4 = new ListViewItem.ListViewSubItem();
            listViewSubItem4.Text = result?.PostedUrl;

            var listViewItem = new ListViewItem(index++.ToString());
            listViewItem.SubItems.Add(listViewSubItem1);
            listViewItem.SubItems.Add(listViewSubItem2);
            listViewItem.SubItems.Add(listViewSubItem3);
            listViewItem.SubItems.Add(listViewSubItem4);

            listView1.Items.Add(listViewItem);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one result.");
                return;
            }

            var selected = listView1.SelectedItems[0];
            if (selected != null)
            {
                var url = selected.SubItems[1].Text;

                if (!string.IsNullOrWhiteSpace(url))
                {
                    Process.Start(url);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select one result.");
                return;
            }

            var selected = listView1.SelectedItems[0];
            if (selected != null)
            {
                var url = selected.SubItems[4].Text;

                if (!string.IsNullOrWhiteSpace(url))
                {
                    Process.Start(url);
                }
            }
        }
    }
}
