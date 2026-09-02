using Quantify.Core;
using Microsoft.AspNetCore.Mvc;
using Quantify.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Quantify.ViewModels;

/// <summary>
/// Controller responsible for user statistics
/// </summary>
public class StatisticsController: Controller
{
    public QuantifyDbContext context;

    public StatisticsController(QuantifyDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Method responsible for certain student statistics such as: last 10 attempts in certain topics based on logs in data base
    /// </summary>
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> DisplayStudentStatistics()
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var taskProgressesGroups = context.StudentTaskProgress.Include(progress => progress.Task).ThenInclude(task => task.Topic).Include(progress => progress.Attempts).Where(progress => progress.UserId == userId).ToList().OrderByDescending(progress => progress.Attempts.Max(attempt => attempt.TimeStarted)).GroupBy(progress => progress.Task.TopicId).Take(10);


        List<TopicProfileStatistics> statistics = new List<TopicProfileStatistics>();
        foreach(var tasksGroupProgresses in taskProgressesGroups)
        {
            TopicProfileStatistics topicStatistics = new TopicProfileStatistics();
            foreach(var taskProgress in tasksGroupProgresses)
            {
                topicStatistics.TopicName = taskProgress.Task.Topic.Name;
                topicStatistics.TopicId = taskProgress.Task.TopicId;
                topicStatistics.ModuleId = taskProgress.Task.Topic.ModuleId;

                
                if(taskProgress.Passed)
                    topicStatistics.TotalRightSolvedTasksCount++;
                
                topicStatistics.TotalTasksCount++;
            }
            topicStatistics.Percentage = float.Round((float)topicStatistics.TotalRightSolvedTasksCount * 100/topicStatistics.TotalTasksCount);
            statistics.Add(topicStatistics);
        }

        return View(statistics);
    }
}