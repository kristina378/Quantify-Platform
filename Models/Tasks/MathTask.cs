using Quantify.Core.Users;
using System.Diagnostics.CodeAnalysis;

namespace Quantify.Core.Models;

public enum DifficultyLevel
{
    EasyPeasy = 0,
    Easy = 1,
    Intermediate = 2,
    UpperIntermediate = 3,
    Hard = 4,
    UltraHard = 5
};

public class MathTask
{
    public long TaskId {get; init;}

    public long TopicId{get; init;}
    public Topic Topic { get; init; } = null!;

    public int PointsCount{get; private set;}
    public DifficultyLevel Level {get; init;}
    public int ExpReward{get; init;}

    //here change init to set 
    public required string Contents {get; init;}
    public User? Author{get; private set;}
    public long? AuthorId { get; init; }

    public List<Answer>? AllAnswers {get; set;}

    protected MathTask(){}

    [SetsRequiredMembers]
    public MathTask(int points, DifficultyLevel level, string content,List<Answer> allAnswers, Topic topic, int expReward = 1)
    {
        PointsCount = points;
        Level = level;
        Contents = content;
        ExpReward = expReward * ((int)level + 1);

        TopicId = topic.TopicId;
        Topic = topic;

        AllAnswers = allAnswers;
    }
}