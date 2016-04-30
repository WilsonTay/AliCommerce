using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

namespace WooCommerce.Automation
{
    public class Browser
    {
        private static Random random = new Random();

        private static IWebDriver instance;
        public static IWebDriver Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FirefoxDriver();
                }

                return instance;
            }
        }

        //public static IWebDriver NewInstance
        //{
        //    get
        //    {
        //        IWebDriver instance = null;

        //        if (random.Next(0, 3) == 2)
        //        {
        //            instance = new FirefoxDriver();
        //        }
        //        else
        //        {
        //            // Create proxy class object
        //            Proxy p = new Proxy();

        //            // Set HTTP Port to 7777
        //            p.HttpProxy = ProxyProvider.GetRandomProxy();

        //            // Create desired Capability object
        //            DesiredCapabilities cap = new DesiredCapabilities();

        //            // Pass proxy object p
        //            cap.SetCapability(CapabilityType.Proxy, p);
        //            instance = new FirefoxDriver(cap);
        //        }

        //        return instance;
        //    }
        //}
    }
}

