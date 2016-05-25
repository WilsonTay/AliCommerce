using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using WooCommerce.Automation;
using WooCommerce.WooCommerce;
using WooCommerceNET;
using WooCommerceNET.WooCommerce;

namespace ConsoleApplication1
{

    class Program
    {
        static void Main(string[] args)
        {
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

            var bot = new BotRunner(
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
                new ConsoleLogger(),
                new ConsoleResultHandler(),
                15 * 60 * 1000);
            bot.Run();
        }
    }
}
