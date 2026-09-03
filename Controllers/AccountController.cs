using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Quantify.Models;
using Quantify.Core.Users;
using Quantify.ViewModels;
using Microsoft.AspNetCore.Identity;
using Quantify.Core.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Quantify.Services.Email;

namespace Quantify.Controllers;

/// <summary>
/// Controller responsible for account registration, login and logout
/// </summary>
public class AccountController : Controller
{
    private readonly QuantifyDbContext _context;
    private readonly IEmailSender _emailSender;

    public AccountController(QuantifyDbContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }


    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult RegisterStudent()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterStudent(RegisterStudentViewModel registration)
    {
        // Early return to display validation errors if user submitted malformed data 
        // (e.g. empty fields, invalid email format)
        if (!ModelState.IsValid)
        {
            return View(registration);
        }


        if(await _context.Users.FirstOrDefaultAsync(user=> user.Email == registration.Email) != null)
        {
            ModelState.AddModelError("Email", "Konto z podanym adresem e-mail już istnieje");
            return View(registration);
        }
        
        if(await _context.Users.FirstOrDefaultAsync(user=> user.NickName == registration.NickName) != null)
        {
            ModelState.AddModelError("NickName", "Podany nickname jest już zajęty");
            return View(registration);
        }
        

        // hash password for more user security
        var hasher = new PasswordHasher<User>();
        string hashedPassword = hasher.HashPassword(null!, registration.Password);
        

        User newUser = new Student(registration.Name, registration.Surname, registration.Email,
                registration.PhoneNumber, registration.NickName, hashedPassword);
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        
        string subject = "Witamy w Quantify Student!";
        string htmlMessage = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #e0e0e0; border-radius: 5px;'>
                <h2 style='color: #2c3e50;'>Witaj {registration.NickName}!</h2>
                <p>Twoje konto w platformie edukacyjnej <strong>Quantify</strong> zostało pomyślnie utworzone.</p>
                <p>Cieszymy się, że do nas dołączasz. Zaloguj się, aby rozpocząć rozwiązywanie zadań.</p>
                <br/>
                <p>Pozdrawiamy,<br/>Zespół Quantify</p>
            </div>";

        try 
        {
            await _emailSender.SendEmailAsync(registration.Email, subject, htmlMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> Błąd wysyłki SMTP: {ex.Message}");
        }
        
        return RedirectToAction("Index", "Home");
    }

    public IActionResult RegisterTutor()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterTutor(RegisterTutorViewModel registration)
    {
        // Early return to display validation errors if user submitted malformed data 
        // (e.g. empty fields, invalid email format)
        if (!ModelState.IsValid)
        {
            return View(registration);
        }

        if(await _context.Users.FirstOrDefaultAsync(user=> user.Email == registration.Email) != null)
        {
            ModelState.AddModelError("Email", "Konto z podanym adresem e-mail już istnieje");
            return View(registration);
        }
        
        if(await _context.Users.FirstOrDefaultAsync(user=> user.NickName == registration.NickName) != null)
        {
            ModelState.AddModelError("NickName", "Podany nickname jest już zajęty");
            return View(registration);
        }


        // hash password for more user security
        var hasher = new PasswordHasher<User>();
        string hashedPassword = hasher.HashPassword(null!, registration.Password);
        
        User newUser = new Tutor(registration.Name, registration.Surname, registration.Email,
                registration.PhoneNumber, registration.NickName, hashedPassword, registration.Experience,
                            registration.EmploymentPlace, registration.AboutTutor);
        

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        
        string subject = "Witamy w Quantify Tutor!";
        string htmlMessage = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #e0e0e0; border-radius: 5px;'>
                <h2 style='color: #2c3e50;'>Witaj {registration.NickName}!</h2>
                <p>Twoje konto w platformie edukacyjnej <strong>Quantify</strong> zostało pomyślnie utworzone.</p>
                <p>Cieszymy się, że do nas dołączasz. Zaloguj się, aby rozpocząć rozwiązywanie zadań.</p>
                <br/>
                <p>Pozdrawiamy,<br/>Zespół Quantify</p>
            </div>";

        try 
        {
            await _emailSender.SendEmailAsync(registration.Email, subject, htmlMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> Błąd wysyłki SMTP: {ex.Message}");
        }
        
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginData)
    {
        // Early return to display validation errors if user submitted malformed data 
        // (e.g. empty fields, invalid email format)
        if (!ModelState.IsValid)
        {
            return View(loginData);
        }

        var row = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginData.Email);
    
        if(row == null)
        {
            ModelState.AddModelError(string.Empty, "Nieprawidłowy e-mail lub hasło.");
            return View(loginData);
        }


        var hasher = new PasswordHasher<User>();
        var checkResult = hasher.VerifyHashedPassword(row,row.PasswordHash,loginData.Password);
        
        if (checkResult == PasswordVerificationResult.Success)
        {
            // here we use identification based on cookies:
            var claims = new List<Claim>();

            //using id in db as identifier that guaranties unique key for identification
            claims.Add(new Claim(ClaimTypes.NameIdentifier, row.Id.ToString()));
            
            //adding user role for frontend
            if(row is Tutor)
                claims.Add(new Claim(ClaimTypes.Role,"Tutor"));
            else if(row is Student)
                claims.Add(new Claim(ClaimTypes.Role,"Student"));
            else
                claims.Add(new Claim(ClaimTypes.Role,"Admin"));
            

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        //for more security the message for wrong password and email is the same
        ModelState.AddModelError(string.Empty, "Nieprawidłowy e-mail lub hasło.");
        return View(loginData);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied() {return View();}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
