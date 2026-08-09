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
            firstModule.AddNewTopic("Pojęcie zbioru",firstTopicMaterials);

            string path2 = Path.Combine(Directory.GetCurrentDirectory(),"Data","SeedData","Topics","DzialaniaNaZbiorach.md");
            string secondTopicMaterials = await File.ReadAllTextAsync(path2);
            firstModule.AddNewTopic("Działania na zbiorach", secondTopicMaterials);

            string path3 = Path.Combine(Directory.GetCurrentDirectory(),"Data","SeedData","Topics","ZbioryLiczbowe.md");
            string thirdTopicMaterials = await File.ReadAllTextAsync(path3);
            firstModule.AddNewTopic("Zbiory liczbowe", thirdTopicMaterials);

            context.Modules.Add(firstModule);
            await context.SaveChangesAsync();
        }
    }
}
