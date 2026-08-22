using System.ComponentModel.DataAnnotations;

namespace Quantify.ViewModels;

public class ShowTaskContentViewModel
{
    [Required]
    public required long TaskId {get; set;}
    [Required]
    public long ModuleId {get;set;}
    [Required]
    public required long TopicId {get;set;}
    // a field for task in order to display
    public long CurrentTaskIndex { get; set; } = 0;

    public long TotalTasksCount {get; set;}

    [Required]
    public required string Contents {get; set;}


    public List<AnswerDisplayViewModel> Answers {get; set;} = new List<AnswerDisplayViewModel>();

    public bool? HasCorrectAnswer {get;set;} = null;
    public int RemainingAttempts {get;set;}
}