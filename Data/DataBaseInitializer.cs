using Microsoft.EntityFrameworkCore;
using Quantify.Core.Models;
using System.Text.Json;
using Markdig;
using System.IO;
using System.Data;

namespace Quantify.Core.Data;

public class ModulesAbsenceException: Exception{}
public class DataBaseInitializer
{
    public async Task InsertLearningMaterials(QuantifyDbContext context)
    {
        // in database there is no modules
        if (! await context.Modules.AnyAsync())
        {
            string pathToData = Path.Combine(Directory.GetCurrentDirectory(),"Data","SeedData","seed-data.json");
            string jsonString = await File.ReadAllTextAsync(pathToData);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var modulesDTO = JsonSerializer.Deserialize<List<ModuleDTO>>(jsonString, options);

            if(modulesDTO == null)
            {
                throw new ModulesAbsenceException();
            }

            //reading data from json and create db models
            foreach(var module in modulesDTO)
            {
                Module newModule = new Module(module.ModuleName, module.Description);
               
                foreach(var topic in module.Topics)
                {
                    string pathToTopicContent = Path.Combine(Directory.GetCurrentDirectory(), topic.PathToContent);
                    string topicContent = await File.ReadAllTextAsync(pathToTopicContent);

                    Topic newTopic = newModule.AddNewTopic(topic.TopicName, topicContent);

                    foreach(var task in topic.Tasks)
                    {   
                        string taskContent;
                        if(task.PathToContent != null)
                        {
                            string pathToTaskContent = Path.Combine(Directory.GetCurrentDirectory(), task.PathToContent);
                            taskContent = await File.ReadAllTextAsync(pathToTaskContent);
                        }
                        else if(task.Content != null)
                        {
                            taskContent = task.Content;
                        }
                        else
                        {
                            throw new DataException("No path to content or content provided!");
                        }
                                
                        List<Answer> answers = new List<Answer>();
                            
                        foreach(var answer in task.Answers)
                        {
                            Answer newAnswer = new Answer()
                            {
                                Content = answer.Content,
                                IsCorrect = answer.IsCorrect
                            };
                            answers.Add(newAnswer);
                        }

                        MathTask newTask = newTopic.AddNewTask(0, 0,taskContent, answers);
                    }
                }
                

                context.Modules.Add(newModule);
            }

            await context.SaveChangesAsync();

        }
    }
}
