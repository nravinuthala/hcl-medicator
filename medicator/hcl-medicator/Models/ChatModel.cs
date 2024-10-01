using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hcl_medicator.Models
{

    public class ChatModel
    {
        public string InputString { get; set; }
        public List<string> Questions { get; set; } // List of questions
        public List<string> Answers { get; set; }   // List of answers

        public ChatModel()
        {
            Questions = new List<string>();
            Answers = new List<string>();
        }
    }


}