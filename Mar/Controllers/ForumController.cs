using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mar.Models;

namespace Mar.Controllers
{
    [Authorize]
    public class ForumController : Controller
    {
        private readonly LoginContext _context;

        // Bra dependancy injection, bättre än Ensurecreated() i db context. Mkt bättre med add-migrations å update-db.
        public ForumController(LoginContext context)
        {
            _context = context;
        }

        public IActionResult Index(int topicId)
        {
            var threads = _context.Threads.Where(t => t.TopicId == topicId).ToList();
            var topic = _context.Topics.FirstOrDefault(t => t.Id == topicId);

            ViewData["TopicId"] = topicId;
            ViewData["TopicTitle"] = (topic != null) ? topic.Title : "Topic not found";
            ViewData["TopicDescription"] = (topic != null) ? topic.Description : "Description not found";

            if (threads.Any()) { return View(threads); }
            else { return View(new List<ThreadModel>()); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ThreadModel thread)
        {
            if (ModelState.IsValid)
            {
                thread.AuthorName = User.Identity.Name.Substring(0, 1).ToUpper() + User.Identity.Name.Substring(1).ToLower();
                thread.Date = DateTime.Now;
                _context.Threads.Add(thread);
                _context.SaveChanges();
                return RedirectToAction("Index", "Forum", new { topicId = thread.TopicId });
            }

            var topic = _context.Topics.FirstOrDefault(t => t.Id == thread.TopicId);
            ViewData["TopicId"] = thread.TopicId;
            ViewData["TopicTitle"] = (topic != null) ? topic.Title : "Topic not found";
            ViewData["TopicDescription"] = (topic != null) ? topic.Description : "Description not found";

            var threads = _context.Threads.Where(t => t.TopicId == thread.TopicId).ToList();
            return View("Index", threads);
        }
    }
}
