namespace Quantify.ViewModels;

public class SummaryTopicViewModel
{
    public long ModuleId {get;set;}
    public long TopicId {get;set;}

    public long TotalTaskCount {get;set;} = 0;
    public long TotalRightSolvedTaskCount {get;set;} = 0;

    public double ScorePercentage => TotalTaskCount == 0 ? 0 : Math.Round((double)TotalRightSolvedTaskCount / TotalTaskCount * 100, 2);
}