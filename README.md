# Quantify

* [Polish version of ReadMe](./docs/pl/README.pl.md)

An educational platform designed to help high school students learn mathematics.

## Technologies & Libraries:
- **Programming languages**: C#
- **Platform**: .NET 10
- **Web application**: ASP.NET Core MVC
- **Frameworks**: Microsoft Entity Framework Core 9.0
- **Database provider**: Pomelo
- **Database**: MySQL 
- **Email Services**: MailKit & MimeKit
- **Content Rendering**: Markdig (Markdown to HTML), KaTeX (Math formulas rendering)
- **Testing**: xUnit & EF Core InMemory Database
- **Build system**: dotnet

## Key Features:
- **Secure Authentication:** Implements secure cookie-based login and robust password hashing to keep user credentials safe.
- **Account Verification:** New users must activate their accounts via a secure, unique link sent to their email address upon registration.
- **Data Transparency & Privacy:** Users have full access to view all personal data the system collects about them. They also have the right and ability to delete their account at any time.
- **Progress Tracking:** Students can monitor their learning progress, statistics, and collected rewards (coins) across various math topics.
- **Content Management:** Dedicated developer/admin accounts have the ability to dynamically add and manage educational materials within the platform.

## System requirements:

To run this app you will need:

1) IDE of your choice (Visual Studio, Rider, or VS Code)
*(Hint: An IDE is not strictly required, but it makes development much easier.)*

2) **.NET SDK 10** environment

3) MySQL data base and possibly client to work with it, for example: MySQLWorkbench
*(Hint: MySQL client is also optional but can be preferable by some users)*

***You can go through whole process in terminal by itself, if you prefer so***


## Instruction how to install for certain OS:

* [Installation process for Windows](docs/en/install-windows.md)
* [Installation process on macOS](docs/en/install-mac.md)
* [Installation process on Linux](docs/en/install-linux.md)


## To run app:

1) Clone and go to repository:
```bash
git clone https://github.com/kristina378/Quantify
cd Quantify
```
2) Create appsettings.json and copy the contents of appsettings.example.json into it
```bash
cp appsettings.example.json appsettings.json
```
*Hint: on Windows use:*
```bash
copy appsettings.example.json appsettings.json
```
3) - In appsettings.json, find the connection string and replace [Pwd=YOUR_PASSWORD;] with your actual MySQL root password.
- Find section ***EmailSettings*** and there replace **YOUR_EMAIL_HERE** (in section ***"SenderEmail"***) with your email
- In field ***Password*** replace **"YOUR_APP_PASSWORD_HERE"** with your generated app password for Gmail ([what is app password and how it works??](https://support.google.com/accounts/answer/185833?hl=en))

*Hint: if you are using something else than Gmail, make sure to change **SmtpServer** to one you are using and **SmtpPort** if necessary*


4) To download necessary packages for app to run (like for example NuGet):
```bash
dotnet restore
```
5) Apply database migrations:
```bash
dotnet ef database update
```
6) Run project:
```bash
dotnet run
```
7) Application will be active at: <http://localhost:5270>

*Hint: If you get error: "Connection isn't private", then try:*
```bash
dotnet dev-certs https --trust
```
*and then run project again by*
```bash
dotnet run
```
