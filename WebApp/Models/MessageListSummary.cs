using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class MessageListSummary
    {
        public string ToEmail { get; set; }
        public int Total { get; set; }
        public List<MessageSummary> MessageSummaries{ get; set; }
    }
}