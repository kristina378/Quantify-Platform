using System.ComponentModel.DataAnnotations;

namespace Quantify.ViewModels;

public class ShowTopicContentViewModel
{
    public long? ModuleId {get; set;}
    [Required]
    public required long TopicId {get; set;}
    [Required]
    public required string Name {get; set;}
    [Required]
    public required string Content {get; set;}

    public bool HasTasks { get; set;}
}