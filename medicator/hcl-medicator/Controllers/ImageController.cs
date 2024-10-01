using hcl_medicator.Filters;
using hcl_medicator.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace hcl_medicator.Controllers
{
    [CustomAuthorize]
    [NoCache]
    public class ImageController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly ImageModel imageModel = new ImageModel();

        public ActionResult ImageIndex()
        {
            return View(imageModel);
        }



        [HttpPost]
        public async Task<ActionResult> SendFileToApi(ImageModel model)
        {
            if (model.UploadedFile != null && model.UploadedFile.ContentLength > 0)
            {
                // Save the uploaded file path
                var filePath = Path.Combine(Server.MapPath("~/App_Data/uploads"), Path.GetFileName(model.UploadedFile.FileName));
                model.UploadedFile.SaveAs(filePath);

                // Create payload with the file path
                var payload = JsonConvert.SerializeObject(new { text = filePath, func = "image" });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");


                var response = await client.PostAsync("http://127.0.0.1:5000/process", content);



                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseString);


                imageModel.text = result.result.ToString();
                imageModel.filePath = filePath;


                return View("ImageIndex", imageModel);
            }
            return View(imageModel);
        }
    }
}