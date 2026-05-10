using FormarkaLMS.Services.Courses.Application.Courses.Commands;
using FormarkaLMS.Services.Courses.Application.Courses.Queries;
using FormarkaLMS.Services.Identity.Application.Users.Queries;
using FormarkaLMS.Services.Learning.Application.Enrollments.Commands;
using FormarkaLMS.Services.Learning.Application.Progress.Commands;
using FormarkaLMS.Services.Learning.Application.Quizzes.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Add OpenAPI/Swagger
builder.Services.AddOpenApi();

// Register DbContexts
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FormarkaLMS.Services.Courses.Infrastructure.Persistence.CoursesDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDbContext<FormarkaLMS.Services.Identity.Infrastructure.Persistence.IdentityDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDbContext<FormarkaLMS.Services.Learning.Infrastructure.Persistence.LearningDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Courses Services
builder.Services.AddScoped<FormarkaLMS.Services.Courses.Domain.Interfaces.ICourseRepository, FormarkaLMS.Services.Courses.Infrastructure.Repositories.CourseRepository>();

// Register Identity Services
builder.Services.AddScoped<FormarkaLMS.Services.Identity.Domain.Interfaces.IUserProfileRepository, FormarkaLMS.Services.Identity.Infrastructure.Repositories.UserProfileRepository>();

// Register Learning Services
builder.Services.AddScoped<FormarkaLMS.Services.Learning.Domain.Interfaces.IEnrollmentRepository, FormarkaLMS.Services.Learning.Infrastructure.Repositories.EnrollmentRepository>();
builder.Services.AddScoped<FormarkaLMS.Services.Learning.Domain.Interfaces.ILessonProgressRepository, FormarkaLMS.Services.Learning.Infrastructure.Repositories.LessonProgressRepository>();
builder.Services.AddScoped<FormarkaLMS.Services.Learning.Domain.Interfaces.IQuizRepository, FormarkaLMS.Services.Learning.Infrastructure.Repositories.QuizRepository>();
builder.Services.AddScoped<FormarkaLMS.Services.Learning.Domain.Interfaces.IQuizResultRepository, FormarkaLMS.Services.Learning.Infrastructure.Repositories.QuizResultRepository>();

// Register MediatR
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(FormarkaLMS.Services.Courses.Application.DTOs.CourseDto).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetUserProfileByIdQuery).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(EnrollStudentCommand).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
