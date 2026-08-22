namespace Quantify.ViewModels;
public class SolveTaskDisplayViewModel
{
    public long TaskId {get; set;}

    public required string Contents {get; set;} = string.Empty;
    public List<AnswerDisplayViewModel>? Answers {get; set;}
}