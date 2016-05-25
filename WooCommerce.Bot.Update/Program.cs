using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooCommerce.Automation;
using WordPressSharp;

namespace WooCommerce.Bot.Update
{
    class Program
    {
        static void Main(string[] args)
        {
            var restAPIKey = ConfigurationManager.AppSettings["wooCommerceRestAPIKey"];
            var restAPISecret = ConfigurationManager.AppSettings["wooCommerceRestAPISecret"];

            var connectionString = "Server=108.167.181.25; Database=wilsonta_wrdp1;Uid=wilsonta_wrdp1;Pwd=ZS1V96yRQe8sA;";
            var runner = new ReviewBotRunner(restAPIKey, restAPISecret, new WordPressClient(), connectionString, new ConsoleLogger());
            runner.Run();
        }
    }
}
