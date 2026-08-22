using System.Linq;

namespace Quantify.Core.Models;

public class LimitApproachCountException: Exception{}
public class Approach
{
    public long ApproachId {get; init;}

    public DateTime TimeStarted{get; private set;}
    public long StudentTaskProgressId { get; init; }

    public bool Passed {get; private set;}= false;

    public StudentTaskProgress? Progress { get; init; }
    
    
    protected Approach()
    {
        TimeStarted = DateTime.UtcNow;
    }
    public Approach(MathTask task, List<Answer> studentAnswers, StudentTaskProgress progress)
    {
        TimeStarted = DateTime.UtcNow;

        List<Answer>? rightAnswers = task.AllAnswers?.FindAll(answer => answer.IsCorrect) ?? new List<Answer>();


        bool fine = rightAnswers.All(answer => studentAnswers.Any(studAnswer => studAnswer.Content == answer.Content));

        if (fine && studentAnswers.Count == rightAnswers.Count)
        {
            Passed = true;
        }
        Progress = progress;
    }
}
