using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class MessageListSummary
    {
        public int Total { get; set; }
        public List<MessageSummary> MessageSummaries{ get; set; }
    }
}