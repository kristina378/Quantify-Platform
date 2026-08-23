using Microsoft.AspNetCore.Identity;
using Quantify.Core.Data;
using Quantify.Core.Users;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;

namespace Quantify.Tests;

public class ProfileDeletionTest
{
    [Fact]
    public void ProfileDeleteTest()
    {
        var options = new DbContextOptionsBuilder<QuantifyDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_Profile_Deletion_Db")
            .Options;

        long userId = 0;
        using (var context = new QuantifyDbContext(options))
        {
            var hasher = new PasswordHasher<User>();
            string hashedPassword = hasher.HashPassword(null!, "12345678");

            Student student = new Student("name", "surname", "email@gmail.com", null, "nickname", hashedPassword);

            context.Users.Add(student);
            context.SaveChanges();

            userId = student.Id;
        }

        using (var context = new QuantifyDbContext(options))
        {
            var userToDelete = context.Users.FirstOrDefault(u => u.Email == "email@gmail.com");
            Assert.NotNull(userToDelete);

            userToDelete.Name = null;
            userToDelete.Surname = null;
            userToDelete.PhoneNumber = null;

            userToDelete.NickName = "deleted" + userToDelete.Id.ToString();
            userToDelete.Email = userToDelete.NickName + "@quantify-math-app.pl";

            userToDelete.PasswordHash = "DELETED_ACCOUNT";
            userToDelete.IsDeleted = true;
            userToDelete.DeletedTime = DateTime.UtcNow;

            context.SaveChanges();
        }

        
        using (var context = new QuantifyDbContext(options))
        {
            var deletedUser = context.Users.FirstOrDefault(u => u.Email == "email@gmail.com");
            Assert.Null(deletedUser);

            deletedUser = context.Users.FirstOrDefault(u => u.Id == userId);
            Assert.NotNull(deletedUser);

            Assert.Null(deletedUser.Name);
            Assert.Null(deletedUser.Surname);
            Assert.Null(deletedUser.PhoneNumber);

            Assert.StartsWith("deleted", deletedUser.NickName);
            Assert.EndsWith(deletedUser.Id.ToString(), deletedUser.NickName);

            Assert.Equal("DELETED_ACCOUNT", deletedUser.PasswordHash);

            Assert.True(deletedUser.IsDeleted);

        }
    }
}