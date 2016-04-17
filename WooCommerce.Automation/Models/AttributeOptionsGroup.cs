using System.Collections.Generic;

namespace WooCommerce.Automation.Models
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
}