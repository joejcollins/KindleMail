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
        
        public ActionResult SignIn(User user)
        {
            ViewBag.Message = "Sign In.";
            if (ModelState.IsValid)
            {
                // Stash the email address in a cookie for the users conven
                var cookie = new HttpCookie("Email")
                {
                    Value = user.Email,
                    Expires = DateTime.Now.AddYears(10)
                };
                Response.Cookies.Add(cookie);
                // Stash both email and the password in the session
                Session.Add("User", user);
                // Show me the messages for the user.
                return this.MessageList();
            }
            else
            {
                // If the cookie isn't null populate the email field
                var emailCookie = Request.Cookies["email"];
                if (emailCookie != null) user.Email = emailCookie.Value;
                return View(user);
            }
        }

        private ActionResult MessageList()
        {
            var viewModel = new MessageListSummary();
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);

               // var password = ConfigurationManager.AppSettings["PASSWORD"];
                var user = (Models.User)Session["User"];
                client.Login(user.Email, user.Password);
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

            return View("MessageList", viewModel);
        }

        public ActionResult MessageRead()
        {
            ViewBag.Message = "Read.";

            return View();
        }

        public ActionResult MessageWrite()
        {
            ViewBag.Message = "Write.";

            return View();
        }
    }
}