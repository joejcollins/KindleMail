using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailKit;
using MailKit.Net.Imap;
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

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                // client.AuthenticationMechanisms.Remove("XOAUTH2");
                var email = ConfigurationManager.AppSettings["EMAIL"];
                var password = ConfigurationManager.AppSettings["PASSWORD"];
                client.Authenticate(email, password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                viewModel.Total = inbox.Count;

                var messageSummaries = new List<MessageSummary>();
                for (var i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);
                    var messageSummary = new Models.MessageSummary()
                    {
                        Title = message.Subject,
                        From = message.From[0].Name
                    };
                    messageSummaries.Add(messageSummary);
                }
                messageSummaries.Reverse(); // Most recent first.
                viewModel.MessageSummaries = messageSummaries;
                client.Disconnect(true);
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