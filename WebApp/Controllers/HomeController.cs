using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;
using ImapX;
using ImapX.Enums;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult SignIn()
        {
            var user = new User();
            // If the cookie isn't null populate the email field
            var emailCookie = Request.Cookies["email"];
            if (emailCookie != null) user.Email = emailCookie.Value;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(User user)
        {
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
                return RedirectToAction("MessageList", "Home", new { type = "Inbox" });
            }
            else
            {
                ViewBag.Message = " - Oops";
                return this.SignIn();
            }
        }

        [HttpGet]
        public ActionResult MessageList(String type)
        {
            var viewModel = new MessageListSummary();
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);
                var user = (Models.User)Session["User"];
                client.Login(user.Email, user.Password);
                client.Behavior.MessageFetchMode = MessageFetchMode.Tiny;
                client.Behavior.AutoPopulateFolderMessages = true; 

                // The Inbox folder is always available on all IMAP servers...
                Folder folder = client.Folders.Inbox;
                switch (type)
                {
                    case "Inbox":
                        folder = client.Folders.Inbox;
                        break;
                    case "Sent":
                        folder = client.Folders.Sent;
                        break;
                }

                // Retrieve storage account from connection string.
                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                // Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve reference to a previously created container.
                var container = blobClient.GetContainerReference("inbox");
                // Retrieve reference to a blob named "myblob".
                var blockBlob = container.GetBlockBlobReference(user.Email);

                using (var memory = new MemoryStream())
                // Create or overwrite the "myblob" blob with contents from a local file.
                using (var writer = new StreamWriter(memory))
                {
                    writer.Write(folder);
                    writer.Flush();
                    memory.Seek(0, SeekOrigin.Begin);
                    blockBlob.UploadFromStream(memory);
                }

                viewModel.Total = folder.Messages.Count();

                var messageSummaries = new List<Models.MessageSummary>();
                for (var i = 0; i < viewModel.Total; i++)
                {
                    var message = folder.Messages[i];
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
            ViewBag.Message = " - " + type;
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