using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hcl_medicator.Models
{
    public class ImageModel
    {
        public HttpPostedFileBase UploadedFile { get; set; }
        public string text { get; set; }
        public string filePath { get; set; }
    }
}