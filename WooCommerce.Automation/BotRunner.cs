using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using OpenQA.Selenium;
using WooCommerce.Automation.Models;

namespace WooCommerce.Automation
{
    public class BotRunner
    {
        private static string chromeUserAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

        private readonly string postTypeStr;
        private readonly IEnumerable<string> included;
        private readonly string priceLowCap;
        private readonly string priceHighCap;
        private readonly string restAPIKey;
        private readonly string restAPISecret;

        private readonly double markUpPercentage;
        private readonly double usdtoMyrCurrencyRate;
        private readonly double productMinPriceAfterConvert;
        private readonly double productBelowMinMarkup;
        private readonly int maxItemPerCategory;
        private readonly int productPauseDelay;

        private readonly int loginPageAppearedPauseDelay;

        private readonly ILogger logger;
        private readonly IResultHandler resultHandler;

        private bool paused = false;

        private readonly string restApiUrl;

        public BotRunner(
            string restApiUrl,
            string postTypeStr,
            IEnumerable<string> included,
            string priceLowCap,
            string priceHighCap,
            string restAPIKey,
            string restAPISecret,
            double markUpPercentage,
            double usdtoMyrCurrencyRate,
            double productMinPriceAfterConvert,
            double productBelowMinMarkup,
            int maxItemPerCategory,
            int productPauseDelay,
            ILogger logger,
            IResultHandler resultHandler,
            int loginPageAppearedPauseDelay)
        {
            this.postTypeStr = postTypeStr;
            this.included = included;
            this.priceLowCap = priceLowCap;
            this.priceHighCap = priceHighCap;
            this.restAPIKey = restAPIKey;
            this.restAPISecret = restAPISecret;
            this.markUpPercentage = markUpPercentage;
            this.usdtoMyrCurrencyRate = usdtoMyrCurrencyRate;
            this.productMinPriceAfterConvert = productMinPriceAfterConvert;
            this.productBelowMinMarkup = productBelowMinMarkup;
            this.maxItemPerCategory = maxItemPerCategory;
            this.productPauseDelay = productPauseDelay;
            this.logger = logger;
            this.resultHandler = resultHandler;
            this.loginPageAppearedPauseDelay = loginPageAppearedPauseDelay;
            this.restApiUrl = restApiUrl;
        }

        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select String.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return String.Join("&", array);
        }

        public void Pause()
        {
            paused = true;
        }

        public void Resume()
        {
            paused = false;
        }

        public void Run()
        {
            PostType postType = PostType.Draft;
            if (postTypeStr.ToLower().Equals("publish"))
            {
                postType = PostType.Publish;
            }

            foreach (var categoryUrl in included)
            {
                try
                {
                    this.logger.Info("Posting category: " + categoryUrl);
                    var currentCategoryUrl = categoryUrl;
                    var totalPosted = 0;
                    for (int page = 1; page < 100; page++)
                    {
                        //currentCategoryUrl = currentCategoryUrl.Replace(".html", "/" + page + ".html");

                        var urlBuilder = new UriBuilder(currentCategoryUrl);
                        var nvc = new NameValueCollection();
                        nvc.Add("shipCountry", "MY");
                        nvc.Add("minPrice", priceLowCap);
                        nvc.Add("maxPrice", priceHighCap);
                        nvc.Add("isFreeShip", "y");
                        nvc.Add("site", "glo");
                        urlBuilder.Query = ToQueryString(nvc);

                        this.logger.Info("Start processing category " + urlBuilder);

                        HtmlNode.ElementsFlags.Remove("form");
                        IWebDriver browser = Browser.Instance;

                        browser.Navigate().GoToUrl(urlBuilder.ToString());
                        var sourceCode = browser.PageSource;
                        //browser.Close();
                        var doc = new HtmlDocument();
                        doc.LoadHtml(sourceCode);
                        //doc = new HtmlWeb { UserAgent = chromeUserAgent }.Load(urlBuilder.ToString());
                        var productAnchors = doc.DocumentNode.Descendants()
                            .Where(
                                a =>
                                    a.Name.Equals("a") && a.Attributes["class"] != null &&
                                    a.Attributes["class"].Value.Trim().Equals("product")
                                    && a.Attributes["href"] != null && a.Attributes["href"].Value.Contains("/item/"))
                            .ToList();

                        this.logger.Info("Category returned " + productAnchors.Count + " urls");

                        foreach (var anchor in productAnchors)
                        {
                            bool loggedPause = false;
                            while (paused)
                            {
                                if (!loggedPause)
                                {
                                    loggedPause = true;
                                    this.logger.Info("Bot paused..");
                                }
                                Thread.Sleep(1000);
                            }

                            var postingLogFormat = "Posting item {0}";

                            var url = anchor.Attributes["href"].Value;

                            if (url.Contains("?"))
                            {
                                url = url.Substring(0, url.IndexOf("?"));
                            }

                            var sku = new Uri(url).Segments.Last().Replace(".html", "");

                            var poster = new AliExpressPoster(
                                    restApiUrl, 
                                    restAPIKey,
                                    restAPISecret,
                                    postType,
                                    markUpPercentage,
                                    usdtoMyrCurrencyRate,
                                    productMinPriceAfterConvert,
                                    productBelowMinMarkup);

                            this.logger.Info(string.Format(postingLogFormat, url));

                            AliExpressPostResult result = null;
                            if (poster.ProductExists(sku))
                            {
                                result = new AliExpressPostResult
                                {
                                    SourceUrl = url,
                                    Success = false,
                                    Reason = "Product already exist"
                                };
                            }
                            else
                            {
                                // result = poster.Post(url);
                            }

                            this.resultHandler.Handle(result);

                            if (result.Success)
                            {
                                totalPosted++;
                            }

                            if (totalPosted > maxItemPerCategory)
                            {
                                break;
                            }

                            if (!result.Success && result.Reason.Equals("Login page"))
                            {
                                this.logger.Warn("Hit into login page. Pausing for " + productPauseDelay);
                                Thread.Sleep(loginPageAppearedPauseDelay);
                            }

                            // Product exist dont pause.
                            if (string.IsNullOrEmpty(result.Reason) ||
                                (!string.IsNullOrEmpty(result.Reason) && !result.Reason.Equals("Product already exist")))
                            {
                                this.logger.Info("Pausing for " + productPauseDelay);
                                Thread.Sleep(productPauseDelay);
                            }
                        }

                        if (totalPosted > maxItemPerCategory)
                        {
                            this.logger.Warn("Reached limit of " + totalPosted + " for category " + currentCategoryUrl);
                            break;
                        }

                        var nextPage = page + 1;
                        var pageElement = doc.DocumentNode.Descendants()
                            .FirstOrDefault(a => a.Name.Equals("div") && a.Attributes["class"] != null
                                                 && a.Attributes["class"].Value.Equals("ui-pagination-navi util-left"));
                        var allPageElement = pageElement.Descendants().Where(a => a.Name.Equals("a"));
                        var nextPageElement = allPageElement.FirstOrDefault(a => a.InnerText.Equals(nextPage.ToString()));

                        currentCategoryUrl = nextPageElement.Attributes["href"].Value;

                        this.logger.Info("Done processing category " + currentCategoryUrl);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.Error("Woops. Pausing 30 minutes..", ex);
                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }

            this.logger.Info("Done posting all products.");
        }
    }
}
