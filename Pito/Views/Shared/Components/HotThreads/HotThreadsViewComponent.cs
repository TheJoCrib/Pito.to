﻿using Pito.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HotThreadsViewComponent : ViewComponent
{
    private readonly LoginContext _context;

    public HotThreadsViewComponent(LoginContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var now = DateTime.UtcNow;

        var threads = _context.Threads
            .Where(t => t.ViewCount >= 1)
            .ToList();

        var hotThreadsByTopic = threads
            .GroupBy(thread => thread.TopicId)
            .SelectMany(group => group
                .Select(thread =>
                {
                    double trendingScore = CalculateTrendingScore(thread, now);

                    return new
                    {
                        Thread = thread,
                        TrendingScore = trendingScore
                    };
                })
                .Take(5)
            )
            .OrderByDescending(item => item.TrendingScore)
            .ToList();

        var topicIds = hotThreadsByTopic.Select(thread => thread.Thread.TopicId).Distinct();

        var topicTitles = _context.Topics
            .Where(topic => topicIds.Contains(topic.Id))
            .ToDictionary(topic => topic.Id, topic => topic.Title);

        foreach (var thread in hotThreadsByTopic)
        {
            if (topicTitles.ContainsKey(thread.Thread.TopicId))
            {
                ViewData[$"ThreadTopic_{thread.Thread.TopicId}"] = topicTitles[thread.Thread.TopicId];
            }
            else
            {
                // Log missing TopicId or handle the case where it's missing
                ViewData[$"ThreadTopic_{thread.Thread.TopicId}"] = "Unknown Author";
            }
            ViewData[$"ThreadScore_{thread.Thread.Id}"] = thread.TrendingScore.ToString("F1");
        }

        return View(hotThreadsByTopic.Select(thread => thread.Thread).ToList());
    }

    private double CalculateTrendingScore(ThreadModel thread, DateTime now)
    {
        return (Math.Log10(thread.ViewCount + 1) * 2) +
            ((now - (thread.Date ?? DateTime.MinValue)).TotalHours / 24);
    }
}