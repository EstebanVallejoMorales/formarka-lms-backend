using FormarkaLMS.Services.Courses.Domain.Interfaces;
using FormarkaLMS.Services.Courses.Infrastructure.Repositories;
using FormarkaLMS.Services.Identity.Domain.Interfaces;
using FormarkaLMS.Services.Identity.Infrastructure.Repositories;
using FormarkaLMS.Services.Learning.Domain.Interfaces;
using FormarkaLMS.Services.Learning.Infrastructure.Repositories;
using FormarkaLMS.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// This will register controllers defined within this Gateway project assembly.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Add Swagger/OpenAPI support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Formarka LMS Gateway API", Version = "v1" });
});

// Configure unified DbContext for all microservices
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories and MediatR handlers for each microservice
// It's important that the MediatR registration scans the correct Application assemblies.

// Courses Service Application Layer & Domain Interfaces
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(FormarkaLMS.Services.Courses.Application.DTOs.CourseDto).Assembly));

// Identity Service Application Layer & Domain Interfaces
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(FormarkaLMS.Services.Identity.Application.Users.Queries.GetUserProfileByIdQuery).Assembly));

// Learning Service Application Layer & Domain Interfaces
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuizResultRepository, QuizResultRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(FormarkaLMS.Services.Learning.Application.Enrollments.Commands.EnrollStudentCommand).Assembly));

// Shared MediatR registration (if any handlers exist in Shared.Application)
// Assuming there might be handlers in Shared, otherwise this could be omitted.
// builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(FormarkaLMS.Shared.Interfaces.IRepository<>).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();

// Use Swagger and SwaggerUI
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Formarka LMS Gateway API v1"));

app.UseAuthorization();

app.MapControllers();

app.Run();
