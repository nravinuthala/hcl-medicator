using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hcl_medicator.Models
{
    public class CartModel
    {

        public int CartID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }

        public decimal Price { get; set; }
        public int SellerID { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}