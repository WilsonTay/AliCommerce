using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WooCommerce.Automation;

namespace WooCommerce.Bot.UI
{
    public partial class Form1 : Form
    {
        private bool paused;
        private bool started;

        private Thread thread;
        private BotRunner bot;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!started)
            {
                button1.Text = "Pause";

                var included = new StreamReader(File.OpenRead("categories.txt")).ReadToEnd().Split('\n').Select(a => a.Trim('\r'));

                var restAPIKey = ConfigurationManager.AppSettings["wooCommerceRestAPIKey"];
                var restAPISecret = ConfigurationManager.AppSettings["wooCommerceRestAPISecret"];
                var priceLowCap = ConfigurationManager.AppSettings["priceLowCap"];
                var priceHighCap = ConfigurationManager.AppSettings["priceHighCap"];
                var maxItemPerCategory = int.Parse(ConfigurationManager.AppSettings["maxItemPerCategory"]);

                var markUpPercentage = Double.Parse(ConfigurationManager.AppSettings["markUpPercentage"]);
                var usdtoMyrCurrencyRate = Double.Parse(ConfigurationManager.AppSettings["usdtoMyrCurrencyRate"]);
                var postTypeStr = ConfigurationManager.AppSettings["postAs"];

                var productPauseDelay = int.Parse(ConfigurationManager.AppSettings["productPauseDelay"]);

                var productMinPriceAfterConvert = Double.Parse(ConfigurationManager.AppSettings["productMinPriceAfterConvert"]);
                var productBelowMinMarkup = Double.Parse(ConfigurationManager.AppSettings["productBelowMinMarkup"]);

                var loginPageAppearedPauseDelay = int.Parse(ConfigurationManager.AppSettings["loginPageAppearedPauseDelay"]);

                var restAPIUrl = ConfigurationManager.AppSettings["wooCommerceRestAPIUrl"];

                bot = new BotRunner(
                             restAPIUrl,
                             postTypeStr,
                             included,
                             priceLowCap,
                             priceHighCap,
                             restAPIKey,
                             restAPISecret,
                             markUpPercentage,
                             usdtoMyrCurrencyRate,
                             productMinPriceAfterConvert,
                             productBelowMinMarkup,
                             maxItemPerCategory,
                             productPauseDelay,
                             new FormLogger(this, this.txtLog),
                             new FormResultHandler(this, this.listView1),
                             loginPageAppearedPauseDelay);

                thread = new Thread(() =>
                {
                    bot.Run();
                });

                thread.Start();
                started = true;
            }
            else
            {
                if (!paused)
                {
                    button1.Text = "Unpause";
                    bot.Pause();
                    paused = true;
                }
                else
                {
                    button1.Text = "Pause";
                    bot.Resume();
                    paused = false;
                }
            }
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

        private void button4_Click(object sender, EventArgs e)
        {
            var random = new Random().Next(0, 999999);
            var fileName = random + ".csv";
            ListViewToCSV.Save(listView1, fileName, true);

            Process.Start(fileName);
        }
    }
}
