namespace Quantify.Core.Models;

public class AddingChancesException: Exception{}
public class StudentTaskProgress
{
    public long StudentTaskProgressId {get; init;}
    public static int LimitCount = 100;

    public long UserId {get; private set;}
    public long TaskId{get; init;}
    public MathTask Task {get; init;} = null!;

    public int ApproachNumber{get; private set;}
    public float Average {get; private set;}
    public bool Passed{get; private set;}
    
    public List<Approach> Attempts {get; init;} = new List<Approach>();

    protected StudentTaskProgress(){}
    public StudentTaskProgress(long userId, MathTask task, List<Answer> studentAnswers)
    {
        ApproachNumber = 1;

        UserId = userId;
        TaskId = task.TaskId;
        Task = task;

        Approach newAttempt = new Approach(task, studentAnswers,this);
        Attempts.Add(newAttempt);

        Average = newAttempt.Passed? task.PointsCount: 0;
        Passed = newAttempt.Passed;
    }

    public Approach AddAnotherApproach(List<Answer> studentAnswers)
    {
        if((++ApproachNumber) < LimitCount)
        {
            Approach nextAttempt = new Approach(this.Task,studentAnswers,this);
            Attempts.Add(nextAttempt);

            Passed = nextAttempt.Passed;
            Average = (Average * (ApproachNumber - 1) + (nextAttempt.Passed? Task.PointsCount: 0))/ApproachNumber;

            // we store in db 3 last attempts and 1 the best for every task progress
            // to prevent too fast filling of db with approaches logs
            var lastAttempts = Attempts.OrderByDescending(attempt => attempt.TimeStarted).Take(3).ToList();
            var bestLastAttempt = Attempts.LastOrDefault(attempt => attempt.Passed);

            Attempts.RemoveAll(attempt => attempt != bestLastAttempt && !lastAttempts.Contains(attempt));


            return nextAttempt;
        }
        else
        {
            throw new LimitApproachCountException();
        }
    }

    public Approach? BestApproach()
    {
        if(Attempts.Count > 0)
        {
            return Attempts.LastOrDefault(attempt => attempt.Passed);
        }
        
        return null;
    }
    // public void AddAnotherChance(int chancesCount)
    // {   
    //     // extra protection for user to not buy infinity amount of Approaches count
    //     chancesCount = chancesCount % (LimitCount - ApproachNumber);
    //     ApproachNumber = ApproachNumber - chancesCount;
    // }
}