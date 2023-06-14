using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace My_Project.Models
{
    public class ShopCart
    {
        public Product prdct { get; set; }
        public int quantity { get; set; }
        public Personnel prsnnl { get; set; }
    }
}