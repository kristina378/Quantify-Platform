using System.Diagnostics.Metrics;
using Quantify.Core.Models;


namespace Quantify.Tests;

public class TestData
{
    public static long ModuleCounter = 0;
    public static long TopicCounter = 0;
    public static int AnswerCounter = 0;
    public static long TaskCounter = 0;

    public static Module CreateSampleModule()
    {
        Module sampleModule = new Module("Module","First Module!")
        {
            ModuleId = ModuleCounter++
        };
        return sampleModule;
    }

    public static Topic CreateSampleTopic(Module module)
    {
        Topic sampleTopic = new Topic("", "")
        {
            Module = module,
            ModuleId = module.ModuleId,
            TopicId = TopicCounter++
        };
        return sampleTopic;
    }

    public static MathTask CreateSampleTask(Topic topic)
    {
        Answer answer1 = new Answer()
        {
            Content = "Not right answer",
            AnswerId = AnswerCounter++,
            IsCorrect = false
        };

        Answer answer2 = new Answer()
        {
            Content = "Right answer!",
            AnswerId = AnswerCounter++,
            IsCorrect = true
        };

        List<Answer> answers = new List<Answer>();
        answers.Add(answer1);
        answers.Add(answer2);

        MathTask sampleTask = new MathTask(0, 0, "task content here", answers, topic)
        {
            TaskId = TaskCounter++,
        };

        return sampleTask;
    }

    public static MathTask CreateSampleMoreComplexTask(Topic topic)
    {
        Answer answer1 = new Answer()
        {
            Content = "Not right answer",
            AnswerId = AnswerCounter++,
            IsCorrect = false
        };

        Answer answer2 = new Answer()
        {
            Content = "Right answer!",
            AnswerId = AnswerCounter++,
            IsCorrect = true
        };

        Answer answer3 = new Answer()
        {
            Content = "Second right answer!",
            AnswerId = AnswerCounter++,
            IsCorrect = true
        };

        Answer answer4 = new Answer()
        {
            Content = "Third right answer!",
            AnswerId = AnswerCounter++,
            IsCorrect = true
        };

        List<Answer> answers = new List<Answer>();
        answers.Add(answer1);
        answers.Add(answer2);
        answers.Add(answer3);
        answers.Add(answer4);


        MathTask sampleTask = new MathTask(0, 0, "task content for more complex task here", answers, topic)
        {
            TaskId = TaskCounter++,
        };

        return sampleTask;
    }
}
