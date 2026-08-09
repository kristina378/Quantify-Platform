using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Quantify.Models;
using Quantify.Core.Data;
using Microsoft.EntityFrameworkCore;
using Quantify.Core.Models;
using Quantify.ViewModels;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Markdig;


namespace Quantify.Controllers;

public class LearningMaterialsController : Controller
{
    private readonly QuantifyDbContext _context;

    public LearningMaterialsController(QuantifyDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> ShowModulesList()
    {
        var modules = await _context.Modules.ToListAsync();
        if(modules == null)
            return NotFound();
        
        var modulesViews = new List<ShowModuleContentViewModel>();

        foreach(var module in modules)
        {
            var newModuleView = new ShowModuleContentViewModel()
            {
                ModuleId = module.ModuleId,
                Name = module.Name,
                Description = module.Description
            };
            modulesViews.Add(newModuleView);
        }

        return View(modulesViews);
    }

    public async Task<IActionResult> ShowModuleContent(long moduleId)
    {
        var module = await _context.Modules.Include(module => module.Topics).FirstOrDefaultAsync(module => module.ModuleId == moduleId);
        if(module == null)
            return NotFound();

        var topics = module.Topics;
        List<ShowTopicContentViewModel>? topicsView = null;
        if(topics != null)
        {   
            topicsView = new List<ShowTopicContentViewModel>();
            foreach(var topic in topics)
            {
                var newTopicView = new ShowTopicContentViewModel()
                {
                    TopicId = topic.TopicId,
                    Name = topic.Name,
                    Content = topic.Content
                };
                topicsView.Add(newTopicView);
            }
        }
        var moduleView = new ShowModuleContentViewModel()
        {
            ModuleId = module.ModuleId,
            Name = module.Name,
            Description = module.Description,
            Topics = topicsView
        };
    
        
        return View(moduleView);
    }

    public async Task<IActionResult> ShowTopicContent(long moduleId, long topicId)
    {
        var module = await _context.Modules.Include(module => module.Topics).FirstOrDefaultAsync(module => module.ModuleId == moduleId);
        var topics = module?.Topics;
        if(topics == null || topics.Count == 0)
        {
            return NotFound();
        }

        var topic = topics.FirstOrDefault(topic => topic.TopicId == topicId);

        if(module == null || topic == null)
            return NotFound();
        

        var pipeline = new MarkdownPipelineBuilder().UseMathematics().Build();

        var tasks = await _context.MathTasks.Where(task => task.TopicId == topic.TopicId).ToListAsync();
        List<ShowTaskContentViewModel>? tasksView = null;
        if(tasks.Count != 0)
        {
            tasksView = new List<ShowTaskContentViewModel>();
            foreach(var task in tasks)
            {
                var newTaskView = new ShowTaskContentViewModel()
                {
                    TaskId = task.TaskId,
                    Contents = task.Contents,
                    PointsCount = task.PointsCount,
                    DifficultyLevel = (int)task.Level
                };

                tasksView.Add(newTaskView);
            }
        }
        
        string htmlContent = Markdown.ToHtml(topic.Content, pipeline);

        var topicView = new ShowTopicContentViewModel()
        {
            TopicId = topic.TopicId,
            ModuleId = moduleId,
            Name = topic.Name,
            Content = htmlContent,
            Tasks = tasksView
        };

        
        return View(topicView);
    }
    
    public async Task<IActionResult> ShowTaskContent(long taskId)
    {
        var task = await _context.MathTasks.Include(task => task.AllAnswers).FirstOrDefaultAsync(task => task.TaskId == taskId);
        if(task == null)
            return NotFound();
        
        List<AnswerViewModel> answers = new List<AnswerViewModel>();
        if(task.AllAnswers != null  && task.AllAnswers.Count != 0)
        {
            foreach(var answer in task.AllAnswers)
            {
                if (!string.IsNullOrWhiteSpace(answer.Content))
                {
                    AnswerViewModel answerView = new AnswerViewModel()
                    {
                        Content = answer.Content,
                        IsCorrect = answer.IsCorrect
                    };
                    answers.Add(answerView);
                }
            }
        }
        var pipeline = new MarkdownPipelineBuilder().UseMathematics().Build();
        string htmlContent = Markdown.ToHtml(task.Contents, pipeline);

        var taskDisplayView = new SolveTaskDisplayViewModel()
        {
            TaskId = task.TaskId,
            Contents = htmlContent,
            Answers = answers
        };
        
        return View(taskDisplayView);
    }

}
