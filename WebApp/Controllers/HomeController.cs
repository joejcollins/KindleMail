using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImapX;
using ImapX.Enums;
using WebApp.Models;
using MessageSummary = WebApp.Models.MessageSummary;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var viewModel = new MessageListSummary();
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);

                var email = ConfigurationManager.AppSettings["EMAIL"];
                var password = ConfigurationManager.AppSettings["PASSWORD"];
                client.Login(email, password);
                client.Behavior.MessageFetchMode = MessageFetchMode.Tiny;
                client.Behavior.AutoPopulateFolderMessages = true; 

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Folders.Inbox;
                viewModel.Total = inbox.Messages.Count();

                var messageSummaries = new List<MessageSummary>();
                for (var i = 0; i < viewModel.Total; i++)
                {
                    var message = inbox.Messages[i];
                    var messageSummary = new Models.MessageSummary()
                    {
                        Title = message.Subject,
                        From = message.From.DisplayName,
                        Read = message.Seen
                    };
                    messageSummaries.Add(messageSummary);
                }
                messageSummaries.Reverse(); // Most recent first.
                viewModel.MessageSummaries = messageSummaries;
                client.Disconnect();
            }

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}