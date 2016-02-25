using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WooCommerceNET;
using WooCommerceNET.WooCommerce;

namespace WooCommerce
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RestAPI rest = new RestAPI("http://www.dealliaomah.com/wc-api/v3/",
                "ck_4cb9634eb054bb803a84a2f9c4dfb60bb858fb54",
                "cs_e19b4dec1eb82d7a5803a46898e512fc96d20eb0");
            WCObject wc = new WCObject(rest);
            var products = wc.GetOrders().Result;

            Console.WriteLine(products);
        }
    }
}
