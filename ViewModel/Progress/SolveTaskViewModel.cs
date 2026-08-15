namespace Quantify.ViewModels;
public class SolveTaskViewModel
{
    public long TaskId {get; set;}
    public long TopicId {get;set;}
    public long ModuleId {get;set;}
    public long CurrentTaskIndex {get; set;}

    public long TotalTasksCount {get;set;}

    public List<long>? UserAnswers {get; set;}
}