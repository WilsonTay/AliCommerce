using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public static string DownloadAndGetLocalUrl(string remoteImageUrl)
        {
            return remoteImageUrl;
            try
            {
                var localPath = "G:\\WooCommerceImages\\";

                var webClient = new WebClient();
                var stream = webClient.OpenRead(remoteImageUrl);

                var bitmap = Bitmap.FromStream(stream);
                var name = Guid.NewGuid().ToString().Replace("-", "") + ".png";
                var path = localPath + name;
                bitmap.Save(path);

                return "http://wilsontay.ddns.net/woocommerceimages/" + name;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception happens for " + remoteImageUrl);
                return "";
            }
        }

        private static string chromeUserAgent =
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Start scanning for categories..");
                    var categoryUrl =
                        "http://www.aliexpress.com/all-wholesale-products.html?spm=2114.11010108.22.1.B0Jzm8";
                    HtmlDocument doc =
                        new HtmlWeb {UserAgent = chromeUserAgent}.Load(categoryUrl);

                    var anchors =
                        doc.DocumentNode.Descendants()
                            .Where(
                                a =>
                                    a.Name.Equals("a") && a.Attributes["href"] != null &&
                                    a.Attributes["href"].Value.Contains("/category/"))
                            .ToList();

                    Console.WriteLine("Retrieved " + anchors.Count + " categories");
                    var randomAnchor = anchors.ElementAt(new Random().Next(0, anchors.Count())).Attributes["href"].Value;

                    Console.WriteLine("Start processing category " + randomAnchor);

                    doc = new HtmlWeb {UserAgent = chromeUserAgent}.Load(randomAnchor);
                    var productAnchors = doc.DocumentNode.Descendants()
                        .Where(
                            a =>
                                a.Name.Equals("a") && a.Attributes["class"] != null &&
                                a.Attributes["class"].Value.Equals("product ")
                                && a.Attributes["href"] != null && a.Attributes["href"].Value.Contains("/item/"))
                        .ToList();

                    Console.WriteLine("Category returned " + productAnchors.Count + " urls");

                    foreach (var anchor in productAnchors)
                    {
                        var postingLogFormat = "Posting item {0}";
                        var postedLogFormat = "Posted item {0}, Status: {1}, Target Url: {2}";
                        var url = anchor.Attributes["href"].Value;
                        Console.WriteLine(postingLogFormat, url);
                        var result = new AliExpressPoster().Generate(url);
                        Console.WriteLine(postedLogFormat, result.SourceUrl, result.Success ? "Success" : "Failed",
                            result?.PostedUrl);

                        Thread.Sleep(15000);
                    }

                    Console.WriteLine("Done processing category " + randomAnchor);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Woops. Pausing 30 minutes.. because of " + ex.StackTrace);
                    Thread.Sleep(TimeSpan.FromMinutes(30));
                }
            }


            Console.Read();

            //return;
            //AliExpressPoster poster = new AliExpressPoster();
            //poster.Generate("http://www.aliexpress.com/item/Chuwi-Vi8-windows-8-1-Android-4-4-Intel-Z3735F-Quad-Core-2GB-32GB-8-IPS/32287056118.html?spm=2114.01010208.3.12.W0FRAp&ws_ab_test=searchweb201556_9,searchweb201602_4_505_506_503_504_10034_10020_502_10001_10002_10017_10010_10005_10006_10011_10003_10021_10004_10022_10009_10008_10018_10019,searchweb201603_2&btsid=192e8479-ab58-4e33-9e6d-47189a408ba2");


            //return;

            //HtmlNode.ElementsFlags.Remove("form");


            //var urls = new List<string>();

            //urls.Add("http://www.aliexpress.com/item/2015-New-Autumn-Fashion-Brand-Casual-Sweater-V-Neck-Striped-Slim-Fit-Knitting-Mens-Sweaters-And/32434244861.html?spm=2114.01010108.3.1.Ga5nt5&ws_ab_test=searchweb201556_9,searchweb201602_4_505_506_2000018_503_504_10034_10020_502_10001_10002_10017_10010_10005_10006_10011_10003_10021_10004_10022_10009_10008_10018_10019,searchweb201603_2&btsid=814ff3bf-2721-4124-b228-cf863788bb19");
            //foreach (var url in urls)
            //{
            //    try
            //    {
            //        HtmlDocument doc =
            //            new HtmlWeb().Load(url);

            //        var imageUrls = new List<string>();
            //        foreach (
            //            HtmlNode link in
            //                doc.DocumentNode.Descendants("li")
            //                    .Where(
            //                        a =>
            //                            a.Attributes.Contains("class") &&
            //                            a.Attributes["class"].Value.Equals("image-nav-item")))
            //        {
            //            var src = link.Descendants("img").ToList()[0].Attributes["src"].Value;
            //            imageUrls.Add(src);
            //        }


            //        var objectId = doc.DocumentNode.Descendants("input")
            //            .FirstOrDefault(
            //                a => a.Attributes["name"] != null && a.Attributes["name"].Value.Equals("objectId"))
            //            .Attributes["value"]
            //            .Value;

            //        var description =
            //            new WebClient().DownloadString("http://desc.aliexpress.com/getDescModuleAjax.htm?productId=" +
            //                                           objectId);
            //        Console.WriteLine(description);


            //        var title =
            //            doc.DocumentNode.Descendants("h1")
            //                .FirstOrDefault(
            //                    a => a.Attributes["class"] != null && a.Attributes["class"].Value.Equals("product-name"))
            //                .InnerText;

            //        RestAPI rest = new RestAPI("http://dealliaomah.com/wc-api/v3/",
            //            "ck_3586593efc9d88b09d62356e76924199b12d1221",
            //            "cs_ec7a0eb857e95790f7bcd1828b37d9fe7542f154");
            //        WCObject wc = new WCObject(rest);

            //        var product = new Product();
            //        product.title = title;

            //        var index1 = description.IndexOf('\'') + 1;
            //        var index2 = description.LastIndexOf('\'');
            //        product.description = description.Substring(index1, index2 - index1);

            //        product.regular_price = 200.00;
            //        product.sale_price = 100.00;
            //        product.in_stock = true;
            //        product.enable_html_description = true;

            //        var imageId = 0;
            //        product.images = imageUrls.Select(a =>
            //        {
            //            var image = new ProductImage();
            //            //image.id = imageId;
            //            image.src = a.Replace("50x50", "640x640");
            //            image.title = "Image " + imageId;
            //            image.position = (short)imageId;

            //            imageId++;
            //            return image;
            //        })
            //            .ToList();

            //        product.categories = new List<string>() { "89" };


            //        var variationDiv =
            //            doc.DocumentNode.Descendants()
            //                .FirstOrDefault(a => a.GetAttributeValue("id", string.Empty).Equals("product-info-sku"));

            //        var attributeOptionsGroupList = new List<AttributeOptionsGroup>();

            //        if (variationDiv != null)
            //        {
            //            product.type = "variable";
            //            var dls = variationDiv.Descendants().Where(a => a.Name.Equals("dl"));

            //            foreach (var dl in dls)
            //            {
            //                var attributeName = dl.Descendants().First(a => a.Name.Equals("dt")).InnerText;
            //                var ul = dl.Descendants().First(a => a.Name.Equals("ul"));

            //                var attributeOptionsGroup = new AttributeOptionsGroup();
            //                attributeOptionsGroup.AttributeName = attributeName;


            //                foreach (var li in ul.Descendants().Where(a => a.Name.Equals("li")))
            //                {
            //                    var anchor = li.Descendants().First(a => a.Name.Equals("a"));
            //                    var anchorTitle = anchor.GetAttributeValue("title", "");
            //                    var attributeValue = "";
            //                    var attributeImage = "";
            //                    if (!string.IsNullOrWhiteSpace(anchorTitle))
            //                    {
            //                        attributeValue = anchorTitle;
            //                        var img = anchor.Descendants().FirstOrDefault(a => a.Name.Equals("img"));

            //                        if (img != null)
            //                        {
            //                            attributeImage = img.GetAttributeValue("bigpic", "");

            //                            Console.WriteLine("IMG:" + attributeImage);
            //                        }
            //                        else
            //                        {
            //                            Console.WriteLine("NOIMG");
            //                        }
            //                    }
            //                    else
            //                    {
            //                        var span = anchor.Descendants().First(a => a.Name.Equals("span"));
            //                        attributeValue = span.InnerText;
            //                    }

            //                    Console.WriteLine(attributeName + "::" + attributeValue);

            //                    var attributeOptions = new SingleAttributeOptions();
            //                    attributeOptions.ImageUrl = DownloadAndGetLocalUrl(attributeImage);
            //                    attributeOptions.AttributeValue = attributeValue;
            //                    attributeOptions.Id = imageId;
            //                    imageId++;
            //                    attributeOptionsGroup.Options.Add(attributeOptions);
            //                }

            //                attributeOptionsGroupList.Add(attributeOptionsGroup);
            //            }
            //        }

            //        var productVariations = new List<ProductVariations>();

            //        for (int i = 0; i < 1; i++)
            //        {
            //            if (attributeOptionsGroupList.Count == 1)
            //            {
            //                var firstGroup = attributeOptionsGroupList.ElementAt(0);
            //                foreach (var firstLevel in firstGroup.Options)
            //                {
            //                    var variation = new ProductVariations();
            //                    variation.id = i;
            //                    variation.regular_price = product.regular_price;
            //                    variation.sale_price = product.sale_price;
            //                    variation.price = product.price;

            //                    if (!string.IsNullOrWhiteSpace(firstLevel.ImageUrl))
            //                    {
            //                        variation.image.Add(new ProductImage
            //                        {
            //                            //position = (short)firstLevel.Id,
            //                            src = firstLevel.ImageUrl,
            //                            title = firstLevel.AttributeValue,
            //                            //id = firstLevel.Id
            //                        });
            //                    }

            //                    variation.attributes.Add(new ProductSpecificAttribute
            //                    {
            //                        name = firstGroup.AttributeName,
            //                        option = firstLevel.AttributeValue,
            //                        slug = firstGroup.AttributeName
            //                    });

            //                    productVariations.Add(variation);
            //                }
            //            }
            //            else if (attributeOptionsGroupList.Count == 2)
            //            {
            //                var firstGroup = attributeOptionsGroupList.ElementAt(0);
            //                var secondGroup = attributeOptionsGroupList.ElementAt(1);

            //                foreach (var firstLevel in firstGroup.Options)
            //                {
            //                    foreach (var secondLevel in secondGroup.Options)
            //                    {
            //                        var variation = new ProductVariations();
            //                        variation.regular_price = product.regular_price;
            //                        variation.sale_price = product.sale_price;
            //                        variation.price = product.price;
            //                        variation.id = i;

            //                        if (!string.IsNullOrWhiteSpace(firstLevel.ImageUrl))
            //                        {
            //                            variation.image.Add(new ProductImage
            //                            {
            //                                //position = (short)firstLevel.Id,
            //                                src = firstLevel.ImageUrl,
            //                                title = firstLevel.AttributeValue,
            //                                //id = firstLevel.Id
            //                            });
            //                        }

            //                        if (!string.IsNullOrWhiteSpace(secondLevel.ImageUrl))
            //                        {
            //                            variation.image.Add(new ProductImage
            //                            {
            //                                //position = (short)firstLevel.Id,
            //                                src = secondLevel.ImageUrl,
            //                                title = secondLevel.AttributeValue,
            //                                //id = firstLevel.Id
            //                            });
            //                        }

            //                        variation.attributes.Add(new ProductSpecificAttribute
            //                        {
            //                            name = firstGroup.AttributeName,
            //                            option = firstLevel.AttributeValue,
            //                            slug = firstGroup.AttributeName
            //                        });
            //                        variation.attributes.Add(new ProductSpecificAttribute
            //                        {
            //                            name = secondGroup.AttributeName,
            //                            option = secondLevel.AttributeValue,
            //                            slug = secondGroup.AttributeName
            //                        });

            //                        productVariations.Add(variation);
            //                    }
            //                }
            //            }
            //        }

            //        product.variations = productVariations;
            //        product.attributes = new List<ProductAttribute>();
            //        var attributeIndex = 0;
            //        foreach (var group in attributeOptionsGroupList)
            //        {
            //            var attribute = new ProductAttribute();
            //            attribute.name = group.AttributeName;
            //            attribute.position = attributeIndex;
            //            attribute.visible = true;
            //            attribute.variation = true;
            //            attribute.options = group.Options.Select(a => a.AttributeValue).ToList();

            //            attributeIndex++;

            //            product.attributes.Add(attribute);
            //        }


            //        var result = wc.PostProduct(product).Result;
            //        Console.WriteLine(result);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex);
            //    }
            //}
            //Console.ReadLine();
        }
    }
}
