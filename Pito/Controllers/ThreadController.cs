using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pito.Models;

namespace Pito.Controllers
{
    [Authorize]
    public class ThreadController : Controller
    {

        private readonly LoginContext _context;

        // Constructor that uses dependency injection to provide an instance of LoginContext
        public ThreadController(LoginContext context)
        {
            _context = context;
        }


        public IActionResult Index(int threadId)
        {
            var thread = _context.Threads.FirstOrDefault(t => t.Id == threadId);

            if (thread != null)
            {
                var topicId = thread.TopicId;
                var topic = _context.Topics.FirstOrDefault(t => t.Id == topicId);

                ViewData["ThreadId"] = threadId;
                ViewData["ThreadTitle"] = thread.Title;
                ViewData["TopicId"] = thread.TopicId;
                ViewData["TopicTitle"] = topic?.Title;

                IncrementViewCount(threadId);

                return View(new List<ThreadModel> { thread });
            }
            else
            {
                return View(new List<ThreadModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ReplyModel reply)
        {
            if (ModelState.IsValid)
            {
                reply.Date = DateTime.Now;
                _context.Replies.Add(reply);
                _context.SaveChanges();
                return RedirectToAction("Index", "Thread", new { threadId = reply.ThreadId });
            }

            var thread = _context.Threads.FirstOrDefault(t => t.Id == reply.ThreadId);

            ViewData["TopicId"] = thread.TopicId;
            ViewData["ThreadId"] = reply.ThreadId;
            ViewData["ThreadTitle"] = (thread != null) ? thread.Title : "Thread not found";

            var threads = _context.Threads.Where(t => t.Id == thread.Id).ToList();
            return View("Index", threads);
        }

        private void IncrementViewCount(int threadId)
        {
            var thread = _context.Threads.FirstOrDefault(t => t.Id == threadId);
            if (thread != null)
            {
                thread.ViewCount++;
                _context.SaveChanges();
            }
        }
    }
}
