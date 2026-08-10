using Quantify.Core.Data;
using Quantify.Core.Models;
using Quantify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quantify.Core.Users;
using System.Security.Claims;

namespace Quantify.Controllers;

public class ProgressController: Controller
{
    protected QuantifyDbContext _context;

    public ProgressController(QuantifyDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CheckAnswer(SolveTaskViewModel userTaskAnswer)
    {
        //extracting data about task from db
        var task = await _context.MathTasks.Include(task => task.AllAnswers).FirstOrDefaultAsync(task => task.TaskId == userTaskAnswer.TaskId);
        if(task == null)
            return NotFound();
        
        var taskAnswers = task.AllAnswers;
        //you can't have a task with at least one field for answer 
        //that why we don't check if answers != null

        
        // we convert answers db models to answer view models
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

        SolveTaskDisplayViewModel displayTask = new SolveTaskDisplayViewModel()
        {   
            TaskId = userTaskAnswer.TaskId,
            Contents = task.Contents,
            Answers = answersViews
        };

        //if user didn't click anything
        // here we need to protect the situation: user can't click on submit until he/she 
        // mark at least one answer
        if(userTaskAnswer.UserAnswers == null || userTaskAnswer.UserAnswers.Count == 0)
        {
            return View("~/Views/LearningMaterials/ShowTaskContent.cshtml", displayTask);
        }
        

        var userId = long.Parse((User.FindFirst(ClaimTypes.NameIdentifier)).Value);
        var student = await _context.Students.Include(student => student.Approaches).ThenInclude(approach => approach.Attempts).FirstOrDefaultAsync(student => student.Id == userId);

        if(student == null)
        {
            //possibility that we couldn't find this user??
        }

        List<Answer> userAnswersDB = new List<Answer>();

        foreach(var answerId in userTaskAnswer.UserAnswers)
        {
            Answer currAnswer = taskAnswers.FirstOrDefault(a => a.AnswerId == answerId);
            if(currAnswer != null)
            {
                userAnswersDB.Add(currAnswer);
            }
        }


        var studentProgress = student!.AddAnotherTaskProgress(task, userAnswersDB);
        var lastApproach = studentProgress.Attempts.Last();

        await _context.SaveChangesAsync();



        if (!lastApproach.Passed)
        {
            //here we need to inform user that task wasn't solve right
            ViewBag.ErrorMessage = "Wrong answer, try again!";
            return View("~/Views/LearningMaterials/ShowTaskContent.cshtml", displayTask);
        }
        //here the case when task solved right : all correct answers were given

        TempData["SuccessMessage"] = "You got it, great job!";
        return RedirectToAction("ShowTaskContent","LearningMaterials", new {taskId = userTaskAnswer.TaskId});
    }
}