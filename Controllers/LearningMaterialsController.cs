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

/// <summary>
/// Controller responsible for showing content learning materials such as module, topic and task
/// </summary>
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

        // here we don't check if we did't found module because of frontend architecture:
        // total modules list -> specific module -> topics in specific module -> specific topic -> all tasks in specific module
        if(topics == null || topics.Count == 0)
        {
            return NotFound();
        }

        var topic = topics.FirstOrDefault(topic => topic.TopicId == topicId);

        if(module == null || topic == null)
            return NotFound();
        

        var hasAnyTasks = await _context.MathTasks.Where(task => task.TopicId == topic.TopicId).AnyAsync();
       
        var pipeline = new MarkdownPipelineBuilder().UseMathematics().Build();
        string htmlContent = Markdown.ToHtml(topic.Content, pipeline);

        var topicView = new ShowTopicContentViewModel()
        {
            TopicId = topic.TopicId,
            ModuleId = moduleId,
            Name = topic.Name,
            Content = htmlContent,
            HasTasks = hasAnyTasks
        };

        
        return View(topicView);
    }
    
    public async Task<IActionResult> ShowTaskContent(long taskId)
    {
        var task = await _context.MathTasks.Include(task => task.AllAnswers).FirstOrDefaultAsync(task => task.TaskId == taskId);
        if(task == null)
            return NotFound();
        
        List<AnswerDisplayViewModel> answers = new List<AnswerDisplayViewModel>();
        if(task.AllAnswers != null  && task.AllAnswers.Count != 0)
        {
            foreach(var answer in task.AllAnswers)
            {
                if (!string.IsNullOrWhiteSpace(answer.Content))
                {
                    AnswerDisplayViewModel answerView = new AnswerDisplayViewModel()
                    {
                        AnswerId = answer.AnswerId,
                        Content = answer.Content
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

    /// <summary>
    /// method responsible for current task content
    /// </summary>
    /// <param name="moduleId"></param>
    /// <param name="topicId"></param>
    /// <param name="wasPreviousAnswerCorrect">parameter that tells us whether: user already completed task earlier and if the answer was right</param>
    /// <param name="currentIndex">task count in total task list in current topic</param>
    /// <returns></returns>
    public async Task<IActionResult> ShowTask(long moduleId, long topicId, bool? wasPreviousAnswerCorrect = null, int currentIndex = 0)
    {
        var allTopicTasks = _context.MathTasks.Include(task => task.AllAnswers).Where(task => task.TopicId == topicId).OrderBy(task => task.TaskId);
        long tasksCount;

        if(allTopicTasks == null || (tasksCount = await allTopicTasks.CountAsync()) == 0)
        {
            return NotFound();
        }

        
        var task = await allTopicTasks.Skip(currentIndex).FirstOrDefaultAsync();
        
        if(task == null || task.AllAnswers == null || task.AllAnswers.Count == 0)
        {
            return NotFound();
        }

        List<AnswerDisplayViewModel> answersViewModels = new List<AnswerDisplayViewModel>();
        foreach(var answer in task.AllAnswers)
        {
            AnswerDisplayViewModel newAnswer = new AnswerDisplayViewModel()
            {
                AnswerId = answer.AnswerId,
                Content = answer.Content
            };
            answersViewModels.Add(newAnswer);
        }

        var pipeline = new MarkdownPipelineBuilder().UseMathematics().Build();
        var htmlContent = Markdown.ToHtml(task.Contents, pipeline);

        var currentTaskDisplay = new ShowTaskContentViewModel()
        {
            TaskId = task.TaskId,
            ModuleId = moduleId,
            TopicId = topicId,
            CurrentTaskIndex = currentIndex,
            Contents = htmlContent,
            TotalTasksCount = tasksCount,
            Answers = answersViewModels,
            RemainingAttempts = 3
        };

        if(wasPreviousAnswerCorrect != null)
        {
            currentTaskDisplay.HasCorrectAnswer = wasPreviousAnswerCorrect;
        }

        return View("ShowTaskContent", currentTaskDisplay);
    }
}
