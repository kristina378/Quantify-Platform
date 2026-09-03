using System.Diagnostics.CodeAnalysis;

namespace Quantify.Core.Users;

public class Admin : User
{
    [SetsRequiredMembers]
    public Admin(string? name, string? surname, string email, string? phoneNumber, string nickName, string passwordHash, string verificationToken)
                :base(name, surname, email, phoneNumber, nickName, passwordHash, verificationToken)
    {
        Permission = Permissions.All;
    }
}