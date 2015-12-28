using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;
using ImapX;
using ImapX.Constants;
using ImapX.Enums;
using ImapX.Exceptions;
using ImapX.Parsing;
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
                client.Behavior.AutoPopulateFolderMessages = false; 

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
                var messages = folder.Search("ALL", MessageFetchMode.Tiny, 20).OrderByDescending(m => m.Date);

                //// Retrieve storage account from connection string.
                //var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                //// Create the blob client.
                //var blobClient = storageAccount.CreateCloudBlobClient();
                //// Retrieve reference to a previously created container.
                //var container = blobClient.GetContainerReference("inbox");
                //// Retrieve reference to a blob named "myblob".
                //var blockBlob = container.GetBlockBlobReference(user.Email);

                //using (var memory = new MemoryStream())
                //// Create or overwrite the "myblob" blob with contents from a local file.
                //using (var writer = new StreamWriter(memory))
                //{
                //    writer.Write(folder);
                //    writer.Flush();
                //    memory.Seek(0, SeekOrigin.Begin);
                //    blockBlob.UploadFromStream(memory);
                //}

                //viewModel.Total = folder.Messages.Count();

                var messageSummaries = new List<Models.MessageSummary>();
                foreach (var message in messages)
                {
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

        //public long[] SearchMessageIds(string query = "ALL", int skip = -1, int count = -1)
        //{
        //    if (_client.SelectedFolder != this && !Select())
        //        throw new OperationFailedException("The folder couldn't be selected for search.");

        //    // Examine the folder before searching
        //    Examine();

        //    if (query.ToUpper() == "ALL" && _client.Behavior.SearchAllNotSupported)
        //        query = "SINCE 0000-00-00";

        //    IList<string> data = new List<string>();
        //    if (!_client.SendAndReceive(string.Format(ImapCommands.Search, query), ref data))
        //        throw new ArgumentException("The search query couldn't be processed");

        //    var result = Expressions.SearchRex.Match(data.FirstOrDefault(Expressions.SearchRex.IsMatch) ?? "");

        //    if (!result.Success)
        //        //throw new OperationFailedException("The data returned from the server doesn't match the requirements");
        //        return new long[0];

        //    IEnumerable<long> results = result.Groups[1].Value.Trim().Split(' ').Select(long.Parse).OrderByDescending(_ => _);
        //    if (skip > 0)
        //        results = results.Skip(skip);
        //    if (count > 0)
        //        results = results.Take(count);
        //    return results.ToArray();
        //}
    }
}