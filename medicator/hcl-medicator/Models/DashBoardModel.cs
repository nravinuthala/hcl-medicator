using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hcl_medicator.Models
{
    public class DashBoardModel
    {
        public int TotalUserCount { get; set; }
        public int TotalCustomerCount { get; set; }
        public int TotalSellerCount { get; set; }
        public int TotalOrdersPlaced { get; set; }
        public int TotalOrdersDelivered { get; set; }
        public int TotalProductsListed { get; set; }
    }
}