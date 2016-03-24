using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooCommerceNET.WooCommerce;

namespace WooCommerce.WooCommerce
{
    public class ProductVariations
    {
        public double? sale_price { get; set; }

        public double? price { get; set; }

        public double? regular_price { get; set; }

        public List<ProductImage> image { get; set; }
         
        public List<ProductSpecificAttribute> attributes { get; set; }

        public int id { get; set; }

        public ProductVariations()
        {
            image = new List<ProductImage>();
            attributes = new List<ProductSpecificAttribute>();
        }
    }
}
