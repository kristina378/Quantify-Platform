using Quantify.Core.Models;
using System.Diagnostics.CodeAnalysis;


namespace Quantify.Core.Users;

public class Student: User
{
    public long CoinsCount{get; private set;}
    public List<StudentTaskProgress> Approaches {get; set;} = new List<StudentTaskProgress>();
    public List<Tutor> Tutors {get; set;} = new List<Tutor>();

    protected Student()
    {
        CoinsCount = 0;
    }

    [SetsRequiredMembers]
    public Student(string? name, string? surname, string email, string? phoneNumber, string nickName, string passwordHash)
                        :base(name, surname, email, phoneNumber, nickName, passwordHash)
    {
        CoinsCount = 0;
    }

    [SetsRequiredMembers]
    public Student(string? name, string? surname, string email, string? phoneNumber, string nickName, string passwordHash,
                 List<Tutor> tutors):base(name, surname, email, phoneNumber, nickName, passwordHash)
    {
        CoinsCount = 0;
        Tutors = tutors;
    }


    public StudentTaskProgress AddAnotherTaskProgress(MathTask task, List<Answer> studentAnswers)
    {
        bool alreadyExists = false;
        StudentTaskProgress studentProgress = null!;
        foreach (StudentTaskProgress progress in Approaches)
        {
            if(progress.TaskId == task.TaskId)
            {
                studentProgress = progress;
                alreadyExists = true;
                break;
            }
        }

        if (alreadyExists)
        {
            studentProgress.AddAnotherApproach(studentAnswers);
            return studentProgress;
        }

        studentProgress = new StudentTaskProgress(this.Id, task, studentAnswers);
        Approaches.Add(studentProgress);

        // if (studentProgress.Passed)
        // {
        //     //AddCoins();
        // }
        return studentProgress;
    }

    public void AddCoins(int CointsCount)
    {
        this.CoinsCount += CointsCount;
    }
}