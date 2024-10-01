using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using hcl_medicator.Models;
using hcl_medicator.Filters;

namespace hcl_medicator.Controllers
{
    [CustomAuthorize]
    [NoCache]
    public class ChatController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly ChatModel chatModel = new ChatModel();

        // GET: Chat
        public ActionResult ChatIndex()
        {
            return View(chatModel);
        }

        [HttpPost]
        public async Task<ActionResult> SendStringToJupyter(ChatModel model)
        {
            var payload = JsonConvert.SerializeObject(new { text = model.InputString , func = "chat"});
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://127.0.0.1:5000/process", content);
            var responseString = await response.Content.ReadAsStringAsync();

            
            var result = JsonConvert.DeserializeObject<dynamic>(responseString);

            
            chatModel.Questions.Add(model.InputString);
            chatModel.Answers.Add(result.result.ToString());

            
            return View("ChatIndex", chatModel);
        }

    }


}
