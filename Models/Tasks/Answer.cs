namespace Quantify.Core.Models;

public class Answer
{
    // here: change type to long
    public int AnswerId { get; init; }

    public long MathTaskId {get; init;}
    public string? Content {get; set;}
    public bool IsCorrect {get; set;}
}