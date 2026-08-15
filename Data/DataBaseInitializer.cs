using Microsoft.EntityFrameworkCore;
using Quantify.Core.Models;
using Markdig;
using System.IO;

namespace Quantify.Core.Data;

public class DataBaseInitializer
{
    public async Task InsertLearningMaterials(QuantifyDbContext context)
    {
        // in database there is no modules
        if (! await context.Modules.AnyAsync())
        {
            Module firstModule = new Module("Zbiory liczbowe","Dział poświęcony zbiorom liczbowym oraz działaniach na nich");

            string path1 = Path.Combine(Directory.GetCurrentDirectory(),"Data","SeedData","Topics","PojecieZbioru.md");
            string firstTopicMaterials = await File.ReadAllTextAsync(path1);

            Topic firstTopic = firstModule.AddNewTopic("Pojęcie zbioru",firstTopicMaterials);
        
            string path11 = Path.Combine(Directory.GetCurrentDirectory(),"Data","SeedData","Tasks","DzialaniaNaZbiorach","Suma.md");
            string path12 = Path.Combine(Directory.GetCurrentDirectory(),"Data","SeedData","Tasks","DzialaniaNaZbiorach","Roznica.md");
            string path13 = Path.Combine(Directory.GetCurrentDirectory(),"Data","SeedData","Tasks","DzialaniaNaZbiorach","CzescWspolna.md");


            List<Answer> answersForTask11 = new List<Answer>();

            Answer answer111 = new Answer();
            answer111.Content = "{1,2,3,2,5,3,7}";
            answer111.IsCorrect = false;

            answersForTask11.Add(answer111);

            Answer answer112 = new Answer();
            answer112.Content = "{1,2,3,5,7}";
            answer112.IsCorrect = true;

            answersForTask11.Add(answer112);

            Answer answer113 = new Answer();
            answer113.Content = "{2,3}";
            answer113.IsCorrect = false;

            answersForTask11.Add(answer113);

            //zadanie na sume zbiorow
            string task11Content = await File.ReadAllTextAsync(path11);
            firstTopic.AddNewTask(0, 0, task11Content, answersForTask11);

            List<Answer> answersForTask12 = new List<Answer>();

            Answer answer121 = new Answer();
            answer121.Content = "{1,2,3,2,5,3,7}";
            answer121.IsCorrect = false;

            answersForTask12.Add(answer121);

            Answer answer122 = new Answer();
            answer122.Content = "{1,2,3,5,7}";
            answer122.IsCorrect = false;

            answersForTask12.Add(answer122);

            Answer answer123 = new Answer();
            answer123.Content = "{1}";
            answer123.IsCorrect = true;

            answersForTask12.Add(answer123);
            //zadanie na roznice zbiorow
            string task12Content = await File.ReadAllTextAsync(path12);
            firstTopic.AddNewTask(0, 0, task12Content, answersForTask12);

            List<Answer> answersForTask13 = new List<Answer>();

            Answer answer131 = new Answer();
            answer131.Content = "{1,2,3}";
            answer131.IsCorrect = false;

            answersForTask13.Add(answer131);

            Answer answer132 = new Answer();
            answer132.Content = "{2,3}";
            answer132.IsCorrect = true;

            answersForTask13.Add(answer132);

            Answer answer133 = new Answer();
            answer133.Content = "{5,7}";
            answer133.IsCorrect = false;

            answersForTask13.Add(answer133);
            //zadanie na czesc wspolna
            string task13Content = await File.ReadAllTextAsync(path13);
            firstTopic.AddNewTask(0, 0, task13Content, answersForTask13);

            string path2 = Path.Combine(Directory.GetCurrentDirectory(),"Data","SeedData","Topics","DzialaniaNaZbiorach.md");
            string secondTopicMaterials = await File.ReadAllTextAsync(path2);
            Topic secondTopic = firstModule.AddNewTopic("Działania na zbiorach", secondTopicMaterials);

            string path3 = Path.Combine(Directory.GetCurrentDirectory(),"Data","SeedData","Topics","ZbioryLiczbowe.md");
            string thirdTopicMaterials = await File.ReadAllTextAsync(path3);
            Topic thirdTopic = firstModule.AddNewTopic("Zbiory liczbowe", thirdTopicMaterials);

            context.Modules.Add(firstModule);
            await context.SaveChangesAsync();
        }
    }
}
