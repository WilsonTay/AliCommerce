using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WooCommerce.WooCommerce
{
    public class ProductAttribute
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int position { get; set; }
        public bool visible { get; set; }
        public List<string> options { get; set; }

        public bool variation { get; set; }

        public ProductAttribute()
        {
            options = new List<string>();
        } 
    }
}
