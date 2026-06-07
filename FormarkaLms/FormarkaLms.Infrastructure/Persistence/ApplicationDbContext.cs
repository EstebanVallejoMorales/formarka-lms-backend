using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FormarkaLms.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Instructor> Instructors => Set<Instructor>();
    public DbSet<Entrepreneurship> Entrepreneurships => Set<Entrepreneurship>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<FormarkaLms.Domain.Entities.Module> Modules => Set<FormarkaLms.Domain.Entities.Module>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonProgress> LessonProgresses => Set<LessonProgress>();
    public DbSet<Deliverable> Deliverables => Set<Deliverable>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizOption> QuizOptions => Set<QuizOption>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
