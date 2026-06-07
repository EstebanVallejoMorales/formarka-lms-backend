using FormarkaLms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Student> Students { get; }
    DbSet<Instructor> Instructors { get; }
    DbSet<Entrepreneurship> Entrepreneurships { get; }
    DbSet<Course> Courses { get; }
    DbSet<FormarkaLms.Domain.Entities.Module> Modules { get; }
    DbSet<Lesson> Lessons { get; }
    DbSet<Resource> Resources { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<LessonProgress> LessonProgresses { get; }
    DbSet<Deliverable> Deliverables { get; }
    DbSet<Quiz> Quizzes { get; }
    DbSet<QuizQuestion> QuizQuestions { get; }
    DbSet<QuizOption> QuizOptions { get; }
    DbSet<QuizAttempt> QuizAttempts { get; }
    DbSet<Comment> Comments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
