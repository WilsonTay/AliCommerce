using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooCommerce.Automation.Models;

namespace WooCommerce.Automation
{
    public interface IResultHandler
    {
        void Handle(AliExpressPostResult result);
    }
}
