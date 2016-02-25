using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WooCommerceNET;
using WooCommerceNET.WooCommerce;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            HtmlNode.ElementsFlags.Remove("form");

            var urls = new List<string>();
            urls.Add("http://www.aliexpress.com/item/Professional-Wired-Gaming-Mouse-7-Button-5500-DPI-LED-Optical-USB-Wired-Computer-Mouse-Mice-Cable/32341324546.html?spm=2114.01010108.3.2.efgskj&ws_ab_test=searchweb201556_7,searchweb201644_1_505_506_503_504_502_10001_10002_10017_10010_10005_10011_10006_10003_10004_10009_10008,searchweb201560_5,searchweb1451318400_-1,searchweb1451318411_6448&btsid=73ee0687-1800-41ad-ad39-95f0a0762cac");
            urls.Add("http://www.aliexpress.com/item/With-Retail-Package-razer-goliathus-gaming-mouse-pad-300-250-2mm-locking-edge-mouse-mat-speed/1928381375.html?spm=2114.01010108.3.12.efgskj&ws_ab_test=searchweb201556_7,searchweb201644_1_505_506_503_504_502_10001_10002_10017_10010_10005_10011_10006_10003_10004_10009_10008,searchweb201560_5,searchweb1451318400_-1,searchweb1451318411_6448&btsid=73ee0687-1800-41ad-ad39-95f0a0762cac");
            urls.Add("http://www.aliexpress.com/item/1Pcs-Mini-Wireless-Optical-Mouse-Mice-USB-Receiver-2-4GHz-Cordless-Scroll-Computer-PC-Optical-Mouse/2047837554.html?spm=2114.01010108.3.21.efgskj&ws_ab_test=searchweb201556_7,searchweb201644_1_505_506_503_504_502_10001_10002_10017_10010_10005_10011_10006_10003_10004_10009_10008,searchweb201560_5,searchweb1451318400_-1,searchweb1451318411_6448&btsid=73ee0687-1800-41ad-ad39-95f0a0762cac");
            urls.Add("http://www.aliexpress.com/item/7-Button-Wired-LED-Optical-USB-Gaming-Mouse-Mice-For-Pro-Gamer-5500-DPI/32325127322.html?spm=2114.01010108.3.39.efgskj&ws_ab_test=searchweb201556_7,searchweb201644_1_505_506_503_504_502_10001_10002_10017_10010_10005_10011_10006_10003_10004_10009_10008,searchweb201560_5,searchweb1451318400_-1,searchweb1451318411_6448&btsid=73ee0687-1800-41ad-ad39-95f0a0762cac");
            urls.Add("http://www.aliexpress.com/item/Lenovo-M20-Wired-Mouse-USB-2-0-Pro-Gaming-Mouse-Optical-Mice-For-Computer-PC-High/32331452023.html?spm=2114.01010108.3.48.efgskj&ws_ab_test=searchweb201556_7,searchweb201644_1_505_506_503_504_502_10001_10002_10017_10010_10005_10011_10006_10003_10004_10009_10008,searchweb201560_5,searchweb1451318400_-1,searchweb1451318411_6448&btsid=73ee0687-1800-41ad-ad39-95f0a0762cac");
            urls.Add("http://www.aliexpress.com/item/New-Arrival-High-quality-mouse-sem-fio-JWFY-Professional-USB-Wired-Gaming-Mouse-7-Buttons-for/32436879177.html?spm=2114.01010108.3.121.efgskj&ws_ab_test=searchweb201556_7,searchweb201644_1_505_506_503_504_502_10001_10002_10017_10010_10005_10011_10006_10003_10004_10009_10008,searchweb201560_5,searchweb1451318400_-1,searchweb1451318411_6448&btsid=73ee0687-1800-41ad-ad39-95f0a0762cac");
            urls.Add("http://www.aliexpress.com/item/Free-Shipping-Mini-USB-2-4Ghz-Snap-in-Transceiver-Optical-Foldable-Folding-Arc-Wireless-Mouse-for/32262617168.html?spm=2114.01010108.3.204.efgskj&ws_ab_test=searchweb201556_7,searchweb201644_1_505_506_503_504_502_10001_10002_10017_10010_10005_10011_10006_10003_10004_10009_10008,searchweb201560_5,searchweb1451318400_-1,searchweb1451318411_6448&btsid=73ee0687-1800-41ad-ad39-95f0a0762cac");


            //urls.Add("http://www.aliexpress.com/item/2015-Original-Micro-USB-Cable-with-Colorful-Nylon-Line-Metal-plug-for-iPhone-6-Plus-5s/32470659404.html?spm=5261.7132366.1998156808.2.PxXdHK");
            //urls.Add("http://www.aliexpress.com/item/NEW-Geneva-Platinum-Watch-Women-PU-Leather-wristwatch-casual-dress-watch-reloj-ladies-relogio-gift-Analog/32280040226.html?spm=5261.7132366.1998156809.30.PxXdHK&sdom=703.123483.111126.0_32280040226");
            //urls.Add("http://www.aliexpress.com/item/Multi-color-Option-Robot-Shape-Android-Micro-USB-To-USB-2-0-Converter-OTG-Adapter-For/2027343968.html?spm=5261.7132366.1998156809.307.PxXdHK&sdom=705.123485.111130.0_2027343968");
            //urls.Add("http://www.aliexpress.com/item/bike-lights-with-no-battery-mountain-road-bike-bicycle-light-lights-LEDS-Tyre-Tire-Valve-Caps/32266640731.html?spm=5261.7132366.1998156809.445.PxXdHK&sdom=706.123486.111132.0_32266640731");
            //urls.Add("http://www.aliexpress.com/item/Pesca-100-pcs-bag-0-63g-5cm-artificial-for-Japan-Shad-Soft-Fishing-Worms-Swimbaits-fishing/1859334833.html?spm=5261.7132366.1998156809.457.PxXdHK&sdom=706.123486.111132.0_1859334833");
            //urls.Add("http://www.aliexpress.com/item/new-winter-baby-Kids-caps-warm-knitting-wool-beanies-twist-Berets-caps-children-thread-Skull-Hat/1533756954.html?spm=5261.7132366.1998156809.709.PxXdHK&sdom=708.123488.111136.0_1533756954");
            //urls.Add("http://www.aliexpress.com/item/Free-shipping-8pcs-STAR-WARS-TMNT-CHIMA-friends-super-hero-minifigures-building-blocks-sets-model-bricks/32318450105.html?spm=5261.7132366.1998156809.711.PxXdHK&sdom=708.123488.111136.0_32318450105");

            // urls.Add(
            //     "http://www.aliexpress.com/item/1-PCS-Universal-Portable-Xiaomi-USB-LED-Light-Fr-PC-Laptop-Power-Bank-partner/32343690810.html");
            //urls.Add(" http://www.aliexpress.com/item/30-Pcs-Rolls-Striping-Tape-Line-Nail-Art-Tips-Decoration-Sticker-Mixed-Colors-Drop-Shipping/32264501527.html");
            // urls.Add(" http://www.aliexpress.com/item/1pc-Mini-LED-Pick-Up-Tool-Telescopic-Magnetic-Magnet-Tool-For-Picking-Up-Nuts-and-Bolts/32529510845.html");
            // urls.Add(" http://www.aliexpress.com/item/Portable-Flexible-USB-Mini-Fan-Xiaomi-popular-portable-USB-Fan-Low-power-For-all-Power-Supply/32592931337.html");
            // urls.Add(" http://www.aliexpress.com/item/2-4GHz-Wireless-optical-mouse-Cordless-Scroll-Computer-PC-Mice-with-USB-Dongle-various-color-gaming/1867425924.html?spm=5261.7132366.1998156809.323.qqyuuo&sdom=705.123485.111130.0_1867425924");

            foreach (var url in urls)
            {
                try
                {
                    HtmlDocument doc =
                        new HtmlWeb().Load(url);

                    var imageUrls = new List<string>();
                    foreach (
                        HtmlNode link in
                            doc.DocumentNode.Descendants("li")
                                .Where(
                                    a =>
                                        a.Attributes.Contains("class") &&
                                        a.Attributes["class"].Value.Equals("image-nav-item")))
                    {
                        var src = link.Descendants("img").ToList()[0].Attributes["src"].Value;
                        imageUrls.Add(src);
                    }


                    var objectId = doc.DocumentNode.Descendants("input")
                        .FirstOrDefault(
                            a => a.Attributes["name"] != null && a.Attributes["name"].Value.Equals("objectId"))
                        .Attributes["value"]
                        .Value;

                    var description =
                        new WebClient().DownloadString("http://desc.aliexpress.com/getDescModuleAjax.htm?productId=" +
                                                       objectId);
                    Console.WriteLine(description);


                    var title =
                        doc.DocumentNode.Descendants("h1")
                            .FirstOrDefault(
                                a => a.Attributes["class"] != null && a.Attributes["class"].Value.Equals("product-name"))
                            .InnerText;

                    RestAPI rest = new RestAPI("http://dealliaomah.com/wc-api/v3/",
                        "ck_4cb9634eb054bb803a84a2f9c4dfb60bb858fb54",
                        "cs_e19b4dec1eb82d7a5803a46898e512fc96d20eb0");
                    WCObject wc = new WCObject(rest);

                    var product = new Product();
                    product.title = title;

                    var index1 = description.IndexOf('\'') + 1;
                    var index2 = description.LastIndexOf('\'');
                    product.description = description.Substring(index1, index2 - index1);
                    //product.id = int.Parse(objectId);
                    // product.price = 200.00;
                    product.regular_price = 200.00;
                    product.sale_price = 100.00;
                    product.in_stock = true;
                    product.enable_html_description = true;

                    var imageId = 0;
                    product.images = imageUrls.Select(a =>
                    {
                        var image = new ProductImage();
                        //image.id = imageId;
                        image.src = a.Replace("50x50", "640x640");
                        image.title = "Image " + imageId;
                        image.position = (short) imageId;

                        imageId++;
                        return image;
                    })
                        .ToList();

                    //var categories = new List<string>();

                    //foreach (var category in categories)
                    //{
                    //    var foundCategory = wc.GetProductCategories().Result.FirstOrDefault(a => a.name.Equals(category));

                    //    if (foundCategory == null)
                    //    {
                    //        var newCategory = new ProductCategory();
                    //        newCategory.name = category;
                    //        newCategory.description = category;

                    //    }

                    product.categories = new List<string>() {"89"};
                    //}

                    var result = wc.PostProduct(product).Result;
                    Console.WriteLine(result);
                } catch (Exception) { }
            }
            Console.ReadLine();
        }
    }
}
