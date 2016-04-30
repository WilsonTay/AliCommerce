using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WooCommerce.Automation.Models;

namespace WooCommerce.Automation
{
    public class ConsoleResultHandler : IResultHandler
    {
        public void Handle(AliExpressPostResult result)
        {
            var postedLogFormat = "Posted item {0}, Status: {1}, Target Url: {2}, Reason: {3}";
            Console.WriteLine(string.Format(postedLogFormat, result.SourceUrl, result.Success ? "Success" : "Failed",
                result?.PostedUrl, result?.Reason));

        }
    }
}
