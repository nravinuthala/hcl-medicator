using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hcl_medicator.Models
{
    public class OrderModel
    {
        public int OrderID { get; set; }
        public int CartID { get; set; }
        public int UserID { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal TotalPrice { get; set; }
        public System.DateTime Date { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public int SellerID { get; set; }
        public string Address { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }
        public virtual User User { get; set; }
    }
}