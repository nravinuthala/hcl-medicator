using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hcl_medicator.Models
{
    public class PrescriptionModel
    {
        public int PrescriptionID { get; set; }
        public int UserID { get; set; }
        public string FilePath { get; set; }
        public string TextContent { get; set; }
    }
}