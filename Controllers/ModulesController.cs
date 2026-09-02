using Quantify.Core.Data;
using Quantify.Core.Models;
using Quantify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

/// <summary>
/// Controller responsible for adding module, topic and tasks (for users with <c>Admin</c> "mode") 
/// in web application in your local Data Base
/// </summary>
public class ModulesController: Controller
{
    private readonly QuantifyDbContext _context;

    public ModulesController(QuantifyDbContext context)
    {
        _context = context;
    }


    [Authorize(Roles = "Admin")]
    public IActionResult AddModule()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    public ActionResult AddTopic(long moduleId)
    {
        AddTopicViewModel topicView = new AddTopicViewModel()
        {
            ModuleId = moduleId
        };
        return View(topicView);
    }

    
    [Authorize(Roles = "Admin")]
    public IActionResult AddTask(long moduleId, long topicId)
    {
        AddTaskViewModel taskView = new AddTaskViewModel(){
            ModuleId = moduleId,
            TopicId = topicId,
            Contents = string.Empty
        };
        return View(taskView);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddModule(AddModuleViewModel module)
    {
        if (!ModelState.IsValid)
        {
            return View(module);
        }

        Module newModule = new Module(module.Name, module.Description);
        _context.Modules.Add(newModule);

        await _context.SaveChangesAsync();


        return RedirectToAction("ShowModuleContent","LearningMaterials", new { moduleId = newModule.ModuleId });
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddTopic(AddTopicViewModel topic)
    {
        if (!ModelState.IsValid)
        {
            return View(topic);
        }

        Module? module = await _context.Modules.Include(module => module.Topics).FirstOrDefaultAsync(module => module.ModuleId == topic.ModuleId);
        if(module == null)
            return NotFound();

        Topic newTopic = new Topic(topic.Name!, topic.Content!);
        module.AddNewTopic(newTopic);

        await _context.SaveChangesAsync();
        

        return RedirectToAction("ShowTopicContent","LearningMaterials", new { moduleId = module.ModuleId, topicId = newTopic.TopicId});
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddTask(AddTaskViewModel task)
    {
         if (!ModelState.IsValid)
        {
            return View(task);
        }
        
        Module? module = await _context.Modules.Include(module => module.Topics)
                    .FirstOrDefaultAsync(module => module.ModuleId == task.ModuleId);
        if(module == null)
            return NotFound();

        Topic? topic = module.Topics.FirstOrDefault(topic => topic.TopicId == task.TopicId);
        if(topic == null)
            return NotFound();
        

        List<Answer> answers = new List<Answer>();

        if(task.AllAnswers != null && task.AllAnswers.Count != 0)
        {
            foreach(var answer in task.AllAnswers)
            {
                if (!string.IsNullOrWhiteSpace(answer.Content))
                {
                    Answer currAnswer = new Answer()
                    {
                        Content = answer.Content,
                        IsCorrect = answer.IsCorrect
                    };
                    answers.Add(currAnswer);
                }
            }
        }
        
        MathTask newTask = new MathTask(task.PointsCount, (DifficultyLevel)task.Level, task.Contents, answers, topic, task.ExpReward);
        topic.AddNewTask(newTask);

        await _context.SaveChangesAsync();

        
        return RedirectToAction("ShowTaskContent", "LearningMaterials", new {taskId = newTask.TaskId});
    }

}