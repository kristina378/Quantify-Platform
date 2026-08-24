using Microsoft.EntityFrameworkCore;
using Quantify.Core.Users;
using Quantify.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Quantify.Core.Data;

public class QuantifyDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Student> Students {get; set;}
    public DbSet<Tutor> Tutors {get; set;}

    public DbSet<MathTask> MathTasks { get; set; }
    public DbSet<StudentTaskProgress> StudentTaskProgress { get; set;}

    public DbSet<Topic> Topic { get; set;}
    public DbSet<Module> Modules { get; set; }

    public QuantifyDbContext(DbContextOptions<QuantifyDbContext> options) : base(options){}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(l => l.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(l => l.NickName)
            .IsUnique();

        modelBuilder.ApplyConfiguration(new AdminConfiguration());
        
        modelBuilder.Entity<Module>()
            .HasIndex(l => l.Name);
        
         modelBuilder.Entity<LearningMaterials>()
             .HasKey(x => x.MaterialId);
        
         modelBuilder.Entity<MathTask>()
             .HasKey(x => x.TaskId);

        // modelBuilder.Entity<MathTask>()
        //     .HasIndex(x => x.TopicId);
        

        // modelBuilder.Entity<Topic>()
        //     .HasIndex(x => x.ModuleId);
        
        modelBuilder.Entity<Topic>()
            .HasIndex(l => l.Name);
        

        // modelBuilder.Entity<StudentTaskProgress>()
        //     .HasIndex(x => x.UserId);
        
    }
}