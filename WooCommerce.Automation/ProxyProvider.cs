using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WooCommerce.Automation
{
    public class ProxyProvider
    {
        public static string[] GetAllProxies()
        {
            return new string[]
            {
                "192.161.162.89:3128",
                "173.234.248.63:3128"
            };
        }

        public static string GetRandomProxy()
        {
            var random = new Random().Next(0,GetAllProxies().Length);

            return GetAllProxies()[random];
        }
    }
}
