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
using WooCommerce.Automation;
using WooCommerce.WooCommerce;
using WooCommerceNET;
using WooCommerceNET.WooCommerce;

namespace ConsoleApplication1
{
    class AttributeOptionsGroup
    {
        public string AttributeName { get; set; }

        public IList<SingleAttributeOptions> Options { get; set; }

        public AttributeOptionsGroup()
        {
            Options = new List<SingleAttributeOptions>();
        }
    }

    class SingleAttributeOptions
    {

        public string AttributeValue { get; set; }
        public string ImageUrl { get; set; }
        public int Id { get; set; }
    }

    class Program
    {

        private static string chromeUserAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return string.Join("&", array);
        }


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

            PostType postType = PostType.Draft;
            if (postTypeStr.ToLower().Equals("publish"))
            {
                postType = PostType.Publish;
            }

            while (true)
            {
                try
                {
                    Console.WriteLine("Start scanning for categories..");
                    var categoryUrl =
                        "http://www.aliexpress.com/all-wholesale-products.html?spm=2114.11010108.22.1.B0Jzm8";
                    HtmlDocument doc =
                        new HtmlWeb { UserAgent = chromeUserAgent }.Load(categoryUrl);

                    var anchors =
                        doc.DocumentNode.Descendants()
                            .Where(
                                a =>
                                    a.Name.Equals("a") && a.Attributes["href"] != null &&
                                    a.Attributes["href"].Value.Contains("/category/") &&
                                    included.Contains(a.InnerText))
                            .ToList();

                    Console.WriteLine("Retrieved " + anchors.Count + " categories");
                    var randomAnchor = anchors.ElementAt(new Random().Next(0, anchors.Count())).Attributes["href"].Value;

                    var totalPosted = 0;
                    for (int page = 1; page < 10; page++)
                    {
                        randomAnchor = randomAnchor.Replace(".html", "/" + page + ".html");

                        var urlBuilder = new UriBuilder(randomAnchor);
                        var nvc = new NameValueCollection();
                        nvc.Add("shipCountry", "MY");
                        nvc.Add("minPrice", priceLowCap);
                        nvc.Add("maxPrice", priceHighCap);
                        nvc.Add("isFreeShip", "y");
                        nvc.Add("site", "glo");
                        urlBuilder.Query = ToQueryString(nvc);

                        Console.WriteLine("Start processing category " + urlBuilder);

                        doc = new HtmlWeb { UserAgent = chromeUserAgent }.Load(urlBuilder.ToString());
                        var productAnchors = doc.DocumentNode.Descendants()
                            .Where(
                                a =>
                                    a.Name.Equals("a") && a.Attributes["class"] != null &&
                                    a.Attributes["class"].Value.Trim().Equals("product")
                                    && a.Attributes["href"] != null && a.Attributes["href"].Value.Contains("/item/"))
                            .ToList();

                        Console.WriteLine("Category returned " + productAnchors.Count + " urls");

                        foreach (var anchor in productAnchors)
                        {
                            var postingLogFormat = "Posting item {0}";
                            var postedLogFormat = "Posted item {0}, Status: {1}, Target Url: {2}";
                            var url = anchor.Attributes["href"].Value;
                            Console.WriteLine(postingLogFormat, url);
                            var result = new AliExpressPoster(restAPIKey, restAPISecret, postType, markUpPercentage, usdtoMyrCurrencyRate).Generate(url);
                            Console.WriteLine(postedLogFormat, result.SourceUrl, result.Success ? "Success" : "Failed",
                                result?.PostedUrl, result?.Reason);

                            if (result.Success)
                            {
                                totalPosted++;
                            }

                            if (totalPosted > maxItemPerCategory)
                            {
                                break;
                            }

                            Thread.Sleep(15000);
                        }

                        if (totalPosted > maxItemPerCategory)
                        {
                            Console.WriteLine("Reached limit of " + totalPosted + " for category " + randomAnchor);
                            break;
                        }

                        Console.WriteLine("Done processing category " + randomAnchor);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Woops. Pausing 30 minutes.. because of " + ex.StackTrace);
                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }

        }
    }
}
