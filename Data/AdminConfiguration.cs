using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quantify.Core.Users;

public class AdminConfiguration :IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        var hasher = new PasswordHasher<User>();
        string hashedPassword = hasher.HashPassword(null!, "AdminDlaDeva2002");

        builder.HasData(new 
        { 
            Id = 1L, 
            NickName = "admin",  
            Email = "kristinadubiaha07@gmail.com", 
            PasswordHash = hashedPassword,
            Permission = Permissions.All ,
            IsDeleted = false,
            EmailIsVerified = true,
            VerificationToken = string.Empty
        });
    }
}