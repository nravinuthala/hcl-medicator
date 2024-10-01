using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hcl_medicator.Models
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get { return Price * Quantity; } }

    }
}