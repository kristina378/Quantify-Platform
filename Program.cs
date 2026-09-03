using Microsoft.EntityFrameworkCore;
using Quantify.Core.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.Extensions.DependencyInjection;
using Quantify.Services.Email;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

        //Connection with MySQL
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<QuantifyDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


        // Pobranie ustawień z appsettings.json
        builder.Services.Configure<Quantify.Services.Email.EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

        // Wstrzyknięcie serwisu (żeby AccountController mógł go użyć)
        builder.Services.AddTransient<Quantify.Services.Email.IEmailSender, Quantify.Services.Email.EmailSender>();


        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            try{
                var context = scope.ServiceProvider.GetRequiredService<QuantifyDbContext>();
                var dbData = new DataBaseInitializer();
                await dbData.InsertLearningMaterials(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } 

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStatusCodePagesWithReExecute("/Home/Error");
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}

