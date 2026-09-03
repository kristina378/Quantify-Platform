namespace Quantify.Services.Email;

public class EmailSettings
{
    public string SmtpServer {get;set;} = "";
    public int SmtpPort {get;set;} = 587;

    public string SenderName {get;set;} = "Unknown sender";
    public required string SenderEmail {get;set;}

    public required string Password {get;set;}

}