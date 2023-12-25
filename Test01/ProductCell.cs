using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test01
{
    internal class ProductCell
    {

       

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Image  Image { get; set; }

        public decimal Price { get; set; }

        public string Manufacturer { get; set; }

        public string Discount { get; set; }
    }
}
