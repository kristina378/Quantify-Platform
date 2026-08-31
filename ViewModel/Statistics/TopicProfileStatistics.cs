namespace Quantify.ViewModels;

public class TopicProfileStatistics
{
    public long TopicId {get;set;}
    public long ModuleId {get;set;}

    public string? TopicName {get;set;}

    public int TotalRightSolvedTasksCount {get;set;} = 0;
    public int TotalTasksCount {get;set;} = 0;

    public float Percentage {get;set;} = 0;
}