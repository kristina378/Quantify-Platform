using System.Diagnostics.CodeAnalysis;

namespace Quantify.Core.Models;

public class Module
{
    public long ModuleId {get; init;}

    public required string Name{get; init;}
    public string? Description {get; set;}

    public List<Topic> Topics {get; private set;} = new List<Topic>();


    protected Module(){}

    [SetsRequiredMembers]
    public Module(string moduleName, string? moduleDescription)
    {
        Name = moduleName;
        Description = moduleDescription;
    }

    public Topic AddNewTopic(string topicName, string topicContent)
    {
        Topic newTopic = new Topic(topicName, topicContent)
        {
            Module = this,
            ModuleId = this.ModuleId
        };
        
        Topics.Add(newTopic);
        return newTopic;
    }
    
    public void AddNewTopic(Topic newTopic)
    {
        Topics.Add(newTopic);
    }
}