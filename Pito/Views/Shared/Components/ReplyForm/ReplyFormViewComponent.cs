using Pito.Models;
using Microsoft.AspNetCore.Mvc;

public class ReplyFormViewComponent : ViewComponent
{
    private readonly LoginContext _context;

    public ReplyFormViewComponent(LoginContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int threadId)
    {
        var replyModel = new ReplyModel { ThreadId = threadId };
        return View(replyModel);
    }
}