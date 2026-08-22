namespace Quantify.Core.Data;

public class TaskDTO
{
    //if task content doesn't contain any formulas and is short than including content in json makes more sens
    // but if not than just path to content to make json contains not too long and complicated
    public string? PathToContent {get;set;}
    public string? Content {get;set;}

    public List<AnswerDTO> Answers {get;set;} = new List<AnswerDTO>();
}