﻿using Pito.Models;
using Microsoft.AspNetCore.Mvc;

public class ThreadFormViewComponent : ViewComponent
{
    private readonly LoginContext _context;

    public ThreadFormViewComponent(LoginContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int topicId)
    {
        var thread = new ThreadModel { TopicId = topicId };
        return View(thread);
    }
}
