using Xunit;
using Quantify.Core.Models;
using System.Threading;
using Quantify.Core.Users;

namespace Quantify.Tests;

public class StudentTaskProgressTest
{  
   

    [Fact]
    public void UserInteractionTest()
    {
        List<Answer> possibleAnswers = new List<Answer>();

        Answer answer1 = new Answer()
        {
            AnswerId = 0,
            MathTaskId = 0,
            Content = "a",
            IsCorrect = true
        };
        possibleAnswers.Add(answer1);

        Answer answer2 = new Answer()
        {
            AnswerId = 1,
            MathTaskId = 0,
            Content = "b",
            IsCorrect = false
        };
        possibleAnswers.Add(answer2);

        Module module = TestData.CreateSampleModule();
        Topic topic = TestData.CreateSampleTopic(module);


        MathTask testTask = new MathTask(0, 0, "Just a test task for testing reasons", possibleAnswers, topic);


        List<Answer> answers = new List<Answer>();
        answers.Add(answer2);

        StudentTaskProgress progress = new StudentTaskProgress(0, testTask, answers);
        Assert.False(progress.Passed);
        Assert.NotEmpty(progress.Attempts);
        Assert.Single(progress.Attempts);

        //here we simulate the user interaction, because unlikely machines, humans are slowly
        Thread.Sleep(100);


        answers.Add(answer1);
        progress.AddAnotherApproach(answers);
        Assert.False(progress.Passed);
        Assert.NotEmpty(progress.Attempts);
        Assert.Equal(2, progress.Attempts.Count);
        Thread.Sleep(100);
        
        for(int i = 0 ; i < 10; i++)
        {
            answers.Clear();
            //here we simulate the user enters incorrect answer
            answers.Add(answer2);
            progress.AddAnotherApproach(answers);

            Assert.False(progress.Passed);
            Assert.NotEmpty(progress.Attempts);
            Assert.Equal(3, progress.Attempts.Count);

            Thread.Sleep(100);

        }

        //Finally user enters right answer
        answers.Clear();
        answers.Add(answer1);
        Approach lastApproach = progress.AddAnotherApproach(answers);
        Assert.True(progress.Passed);
        Assert.Equal(3, progress.Attempts.Count);
        Assert.Equal(lastApproach, progress.BestApproach());
    }
}
