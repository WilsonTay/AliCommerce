using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using WooCommerce.WooCommerce;
using WooCommerceNET;
using WooCommerceNET.WooCommerce;

namespace WooCommerce.Automation
{
    public class WooCommerceResult
    {
        public WooCommerceResultProduct product { get; set; }
    }

    public class WooCommerceResultProduct
    {
        public string permalink { get; set; }
        public string title { get; set; }
    }
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

    public class AliExpressPostResult
    {
        public bool Success { get; set; }
        public string PostedUrl { get; set; }
        public string SourceUrl { get; set; }
        public string Name { get; set; }
    }

    public class AliExpressPoster
    {
        private ProductCategory GetCategory(WCObject wc, string categoryName, string parentCategoryName)
        {
            var categories = wc.GetProductCategories().Result;
            var category = categories.FirstOrDefault(a => WebUtility.HtmlDecode(a.name).Equals(categoryName));
            ProductCategory parentCategory = null;

            if (!string.IsNullOrWhiteSpace(parentCategoryName))
            {
                parentCategory = categories.First(a => WebUtility.HtmlDecode(a.name).Equals(parentCategoryName));
            }

            if (category == null)
            {
                var newcategory = new ProductCategory
                {
                    name = categoryName,
                    count = 0,
                    description = categoryName,
                    id = 0,
                    slug = categoryName
                };

                if (parentCategory != null)
                {
                    newcategory.parent = parentCategory.id;
                }

                var result = wc.PostProductCategory(newcategory).Result;
                category = categories.FirstOrDefault(a => a.name.Equals(categoryName));
                return category;

            }
            else
            {
                return category;
            }
        }

        private static string chromeUserAgent =
         "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";

        public AliExpressPostResult Generate(string url)
        {
            try
            {
                RestAPI rest = new RestAPI("http://dealliaomah.com/wc-api/v3/",
                 "ck_3586593efc9d88b09d62356e76924199b12d1221",
                 "cs_ec7a0eb857e95790f7bcd1828b37d9fe7542f154");
                WCObject wc = new WCObject(rest);

                HtmlNode.ElementsFlags.Remove("form");

                HtmlDocument doc =
                    new HtmlWeb
                    {
                        UserAgent = chromeUserAgent
                    }.Load(url);


                var categoriesElement =
                    doc.DocumentNode.Descendants()
                        .FirstOrDefault(
                            a => a.Attributes.Contains("class") && a.Attributes["class"].Value.Equals("ui-breadcrumb"));

                var categories = categoriesElement.Descendants().Where(
                    a => a.Name.Equals("a")
                    && !a.InnerText.Equals("All Categories")
                    && !a.InnerText.Equals("Home")).Select(a => WebUtility.HtmlDecode(a.InnerText));

                var categoryName = "";
                var parentCategoryName = "";

                foreach (var category in categories)
                {
                    var model = GetCategory(wc, category, parentCategoryName);
                    parentCategoryName = category;

                    categoryName = model.id.ToString();
                }
                //Console.WriteLine(categories);
                //return null;

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

                var title =
                    doc.DocumentNode.Descendants("h1")
                        .FirstOrDefault(
                            a => a.Attributes["class"] != null && a.Attributes["class"].Value.Equals("product-name"))
                        .InnerText;



                var product = new Product();
                product.title = title;
                product.categories = new List<string>();
                product.categories.Add(categoryName);

                var index1 = description.IndexOf('\'') + 1;
                var index2 = description.LastIndexOf('\'');
                product.description = description.Substring(index1, index2 - index1);

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
                    image.position = (short)imageId;

                    imageId++;
                    return image;
                })
                    .ToList();

                //product.categories = new List<string>() { "89" };


                var variationDiv =
                    doc.DocumentNode.Descendants()
                        .FirstOrDefault(a => a.GetAttributeValue("id", string.Empty).Equals("product-info-sku"));

                var attributeOptionsGroupList = new List<AttributeOptionsGroup>();

                if (variationDiv != null)
                {
                    product.type = "variable";
                    var dls = variationDiv.Descendants().Where(a => a.Name.Equals("dl"));

                    foreach (var dl in dls)
                    {
                        var attributeName = dl.Descendants().First(a => a.Name.Equals("dt")).InnerText;
                        var ul = dl.Descendants().First(a => a.Name.Equals("ul"));

                        var attributeOptionsGroup = new AttributeOptionsGroup();
                        attributeOptionsGroup.AttributeName = attributeName;


                        foreach (var li in ul.Descendants().Where(a => a.Name.Equals("li")))
                        {
                            var anchor = li.Descendants().First(a => a.Name.Equals("a"));
                            var anchorTitle = anchor.GetAttributeValue("title", "");
                            var attributeValue = "";
                            var attributeImage = "";
                            if (!string.IsNullOrWhiteSpace(anchorTitle))
                            {
                                attributeValue = anchorTitle;
                                var img = anchor.Descendants().FirstOrDefault(a => a.Name.Equals("img"));

                                if (img != null)
                                {
                                    attributeImage = img.GetAttributeValue("bigpic", "");
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                var span = anchor.Descendants().First(a => a.Name.Equals("span"));
                                attributeValue = span.InnerText;
                            }

                            var attributeOptions = new SingleAttributeOptions();
                            attributeOptions.ImageUrl = attributeImage;
                            attributeOptions.AttributeValue = attributeValue;
                            attributeOptions.Id = imageId;
                            imageId++;
                            attributeOptionsGroup.Options.Add(attributeOptions);
                        }

                        attributeOptionsGroupList.Add(attributeOptionsGroup);
                    }
                }

                var productVariations = new List<ProductVariations>();

                for (int i = 0; i < 1; i++)
                {
                    if (attributeOptionsGroupList.Count == 1)
                    {
                        var firstGroup = attributeOptionsGroupList.ElementAt(0);
                        foreach (var firstLevel in firstGroup.Options)
                        {
                            var variation = new ProductVariations();
                            variation.id = i;
                            variation.regular_price = product.regular_price;
                            variation.sale_price = product.sale_price;
                            variation.price = product.price;

                            if (!string.IsNullOrWhiteSpace(firstLevel.ImageUrl))
                            {
                                variation.image.Add(new ProductImage
                                {
                                    //position = (short)firstLevel.Id,
                                    src = firstLevel.ImageUrl,
                                    title = firstLevel.AttributeValue,
                                    //id = firstLevel.Id
                                });
                            }

                            variation.attributes.Add(new ProductSpecificAttribute
                            {
                                name = firstGroup.AttributeName,
                                option = firstLevel.AttributeValue,
                                slug = firstGroup.AttributeName
                            });

                            productVariations.Add(variation);
                        }
                    }
                    else if (attributeOptionsGroupList.Count == 2)
                    {
                        var firstGroup = attributeOptionsGroupList.ElementAt(0);
                        var secondGroup = attributeOptionsGroupList.ElementAt(1);

                        foreach (var firstLevel in firstGroup.Options)
                        {
                            foreach (var secondLevel in secondGroup.Options)
                            {
                                var variation = new ProductVariations();
                                variation.regular_price = product.regular_price;
                                variation.sale_price = product.sale_price;
                                variation.price = product.price;
                                variation.id = i;

                                if (!string.IsNullOrWhiteSpace(firstLevel.ImageUrl))
                                {
                                    variation.image.Add(new ProductImage
                                    {
                                        //position = (short)firstLevel.Id,
                                        src = firstLevel.ImageUrl,
                                        title = firstLevel.AttributeValue,
                                        //id = firstLevel.Id
                                    });
                                }

                                if (!string.IsNullOrWhiteSpace(secondLevel.ImageUrl))
                                {
                                    variation.image.Add(new ProductImage
                                    {
                                        //position = (short)firstLevel.Id,
                                        src = secondLevel.ImageUrl,
                                        title = secondLevel.AttributeValue,
                                        //id = firstLevel.Id
                                    });
                                }

                                variation.attributes.Add(new ProductSpecificAttribute
                                {
                                    name = firstGroup.AttributeName,
                                    option = firstLevel.AttributeValue,
                                    slug = firstGroup.AttributeName
                                });
                                variation.attributes.Add(new ProductSpecificAttribute
                                {
                                    name = secondGroup.AttributeName,
                                    option = secondLevel.AttributeValue,
                                    slug = secondGroup.AttributeName
                                });

                                productVariations.Add(variation);
                            }
                        }
                    }
                }


                product.variations = productVariations;
                product.attributes = new List<ProductAttribute>();
                var attributeIndex = 0;
                foreach (var group in attributeOptionsGroupList)
                {
                    var attribute = new ProductAttribute();
                    attribute.name = group.AttributeName;
                    attribute.position = attributeIndex;
                    attribute.visible = true;
                    attribute.variation = true;
                    attribute.options = group.Options.Select(a => a.AttributeValue).ToList();

                    attributeIndex++;

                    product.attributes.Add(attribute);
                }

                var resultStr = wc.PostProduct(product).Result;
                var result = JsonConvert.DeserializeObject<WooCommerceResult>(resultStr);
                Console.WriteLine(result);

                return new AliExpressPostResult
                {
                    PostedUrl = result.product.permalink,
                    SourceUrl = url,
                    Name = result.product.title,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new AliExpressPostResult
                {
                    SourceUrl = url,
                    Success = false
                };
            }
        }
    }
}
