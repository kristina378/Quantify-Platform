using Quantify.Core.Data;
using Quantify.Core.Models;
using Quantify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quantify.Core.Users;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;

namespace Quantify.Controllers;

/// <summary>
/// Controller responsible for user progress: checking specific task, and summaries test for current topic
/// </summary>
public class ProgressController: Controller
{
    protected QuantifyDbContext _context;

    public ProgressController(QuantifyDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Method responsible for checking whether user answered right or not (or not answered at all) and redirect to next task in current topic
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CheckAnswer(SolveTaskViewModel userTaskAnswer)
    {
        var task = await _context.MathTasks.Include(task => task.AllAnswers).FirstOrDefaultAsync(task => task.TaskId == userTaskAnswer.TaskId);
        if(task == null)
            return NotFound();
        
        var taskAnswers = task.AllAnswers;
        //you can't have a task with at least one field for answer 
        //that why we don't check if answers != null

        
        List<AnswerDisplayViewModel> answersViews = new List<AnswerDisplayViewModel>();
        foreach(var answer in taskAnswers!)
        {
            AnswerDisplayViewModel answerView = new AnswerDisplayViewModel()
            {
                Content = answer.Content,
                AnswerId = answer.AnswerId
            };
            answersViews.Add(answerView);
        }

        ShowTaskContentViewModel displayTask = new ShowTaskContentViewModel()
        {   
            TaskId = userTaskAnswer.TaskId,
            TopicId = userTaskAnswer.TopicId,
            TotalTasksCount = userTaskAnswer.TotalTasksCount,
            CurrentTaskIndex = userTaskAnswer.CurrentTaskIndex,
            Contents = task.Contents,
            Answers = answersViews
        };

        //if user didn't click anything
        // here we need to protect the situation: user can't click on submit until he/she 
        // mark at least one answer
        if(userTaskAnswer.UserAnswers == null || userTaskAnswer.UserAnswers.Count == 0)
        {
            TempData["ErrorMessage"] = "You must select at least one answer!";
            return RedirectToAction("ShowTask","LearningMaterials", new {moduleId = userTaskAnswer.ModuleId, topicId = userTaskAnswer.TopicId, currentIndex = userTaskAnswer.CurrentTaskIndex});
        }
        

        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userId = long.Parse(claimValue!);
        var student = await _context.Students.Include(student => student.Approaches).ThenInclude(approach => approach.Attempts).FirstOrDefaultAsync(student => student.Id == userId);

        if(student == null) return Unauthorized();

        List<Answer> userAnswersDB = new List<Answer>();

        foreach(var answerId in userTaskAnswer.UserAnswers)
        {
            Answer? currAnswer = taskAnswers.FirstOrDefault(a => a.AnswerId == answerId);
            if(currAnswer != null)
            {
                userAnswersDB.Add(currAnswer);
            }
        }


        var studentProgress = student!.AddAnotherTaskProgress(task, userAnswersDB);
        var lastApproach = studentProgress.Attempts.Last();

        await _context.SaveChangesAsync();


        displayTask.HasCorrectAnswer = lastApproach.Passed;
        displayTask.RemainingAttempts = StudentTaskProgress.LimitCount - studentProgress.ApproachNumber;

        if (!lastApproach.Passed)
        {
            TempData["ErrorMessage"] = "Wrong answer, try again!";
        }

        //here the case when task solved right : all correct answers were given
        else
        {
            TempData["SuccessMessage"] = "You got it, great job!";
        }

        //it was the last task in current topic test
        if(userTaskAnswer.CurrentTaskIndex + 1 >= userTaskAnswer.TotalTasksCount)
        {
            return RedirectToAction("ShowTask","LearningMaterials", new {moduleId = userTaskAnswer.ModuleId, topicId = userTaskAnswer.TopicId, wasPreviousAnswerCorrect = lastApproach.Passed, currentIndex = userTaskAnswer.CurrentTaskIndex});
        }

        return RedirectToAction("ShowTask","LearningMaterials", new {moduleId = userTaskAnswer.ModuleId, topicId = userTaskAnswer.TopicId, currentIndex = userTaskAnswer.CurrentTaskIndex + 1, taskId = userTaskAnswer.TaskId});
    }

    [Authorize]
    public async Task<IActionResult> SummaryTopic(long moduleId, long topicId)
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userId = long.Parse(claimValue!);
        var user = await _context.Students.Include(student => student.Approaches).FirstOrDefaultAsync(student => student.Id == userId);

        if(user == null || user.Approaches == null || user.Approaches.Count == 0)
        {
            return NotFound();
        }


        var allTopicTasksId = (await _context.MathTasks.Where(task => task.TopicId == topicId).OrderBy(task => task.TaskId).ToListAsync()).Select(t => t.TaskId).ToList();
        if(allTopicTasksId == null)
        {
            return NotFound();
        }


        SummaryTopicViewModel summaryTest = new SummaryTopicViewModel()
        {
            ModuleId = moduleId,
            TopicId = topicId,
            TotalTaskCount = allTopicTasksId.Count
        };

        foreach(var taskId in allTopicTasksId)
        {
            var approach = user.Approaches.LastOrDefault(approach => approach.TaskId == taskId);
            if(approach == null)
            {
                continue;
            }

            if(approach.Passed)
                summaryTest.TotalRightSolvedTaskCount += 1;
        }


        return View(summaryTest);
    }
}
