using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hcl_medicator.Models
{
    public class SellerDashboard
    {

        public int TotalOrdersPlaced { get; set; }
        public int TotalOrdersDelivered { get; set; }
        public int TotalProductsListed { get; set; }
    }
}