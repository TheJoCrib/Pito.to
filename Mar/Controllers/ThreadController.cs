using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mar.Models;

namespace Mar.Controllers
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
                reply.AuthorName = User.Identity.Name.Substring(0, 1).ToUpper() + User.Identity.Name.Substring(1).ToLower(); // Capture the current user's username
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


        public IActionResult MyThreads()
        {
            var currentUser = User.Identity.Name.Substring(0, 1).ToUpper() + User.Identity.Name.Substring(1).ToLower();
            var threads = _context.Threads.Where(t => t.AuthorName == currentUser).ToList();
            return View(threads);
        }

        public IActionResult Edit(int id)
        {
            var thread = _context.Threads.FirstOrDefault(t => t.Id == id && t.AuthorName == User.Identity.Name.Substring(0, 1).ToUpper() + User.Identity.Name.Substring(1).ToLower());
            if (thread == null)
            {
                return NotFound();
            }
            return View(thread);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ThreadModel updatedThread)
        {
            var thread = _context.Threads.FirstOrDefault(t => t.Id == id && t.AuthorName == User.Identity.Name.Substring(0, 1).ToUpper() + User.Identity.Name.Substring(1).ToLower());
            if (thread == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                thread.Title = updatedThread.Title;
                thread.Text = updatedThread.Text;
                _context.SaveChanges();
                return RedirectToAction("MyThreads");
            }
            return View(updatedThread);

        }

        public IActionResult Delete(int id)
        {
            var thread = _context.Threads.FirstOrDefault(t => t.Id == id && t.AuthorName == User.Identity.Name.Substring(0, 1).ToUpper() + User.Identity.Name.Substring(1).ToLower());
            if (thread != null)
            {
                _context.Threads.Remove(thread);
                _context.SaveChanges();
                return RedirectToAction("MyThreads");
            }
            return NotFound();
        }


    }
}
