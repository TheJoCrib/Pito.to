using Pito.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class TopicsViewComponent : ViewComponent
{
    private readonly LoginContext _context;

    public TopicsViewComponent(LoginContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var topics = await _context.Topics.ToListAsync();

        if (topics == null || topics.Count == 0)
        {
            return View(new List<TopicModel>());
        }

        return View(topics);
    }
}