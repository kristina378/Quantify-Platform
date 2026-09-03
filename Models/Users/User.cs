using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;


namespace Quantify.Core.Users;

public enum Permissions
{
    All = 1,
    None = 0
}

public abstract class User
{
    public long Id {get; private set;}
    public Permissions Permission { get; protected set;}

    public string? Name {get; set;}
    public string? Surname {get; set;}

    public required string Email {get; set;}
    public bool EmailIsVerified {get;set;} = false;
    public string? VerificationToken { get; protected set; }

    public string? PhoneNumber {get; set;}
    public required string NickName {get; set;}

    public required string PasswordHash { get; set; }

    public bool IsDeleted {get; set;}
    public DateTime? DeletedTime {get; set;}

    protected User(){}

    [SetsRequiredMembers]
    public User(string? name, string? surname, string email, string? phoneNumber, string nickName, string passwordHash, string verificationToken)
    {
        Name = name;
        Surname = surname;

        Email = email;
        PhoneNumber = phoneNumber;
        NickName = nickName;

        PasswordHash = passwordHash;
        IsDeleted = false;

        EmailIsVerified = false;
        VerificationToken = verificationToken;
    }

    public void ConfirmEmail()
    {
        EmailIsVerified = true;
        VerificationToken = null;
    }
}