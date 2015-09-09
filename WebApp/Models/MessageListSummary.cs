using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MailKit;

namespace WebApp.Models
{
    public class MessageListSummary
    {
        public int Total { get; set; }
        public List<MessageSummary> MessageSummaries{ get; set; }
    }
}