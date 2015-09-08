using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MailKit;
using MailKit.Net.Imap;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //using (var client = new ImapClient())
            //{
            //    client.Connect("imap.friends.com", 993, true);

            //    // Note: since we don't have an OAuth2 token, disable
            //    // the XOAUTH2 authentication mechanism.
            //    client.AuthenticationMechanisms.Remove("XOAUTH2");

            //    client.Authenticate("joejcollins@gmail.com", "password");

            //    // The Inbox folder is always available on all IMAP servers...
            //    var inbox = client.Inbox;
            //    inbox.Open(FolderAccess.ReadOnly);

            //    Console.WriteLine("Total messages: {0}", inbox.Count);
            //    Console.WriteLine("Recent messages: {0}", inbox.Recent);

            //    for (int i = 0; i < inbox.Count; i++)
            //    {
            //        var message = inbox.GetMessage(i);
            //        Console.WriteLine("Subject: {0}", message.Subject);
            //    }

            //    client.Disconnect(true);
            //}

            return View();
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