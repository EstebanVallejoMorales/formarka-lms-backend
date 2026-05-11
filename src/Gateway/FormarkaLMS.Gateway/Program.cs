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

// Configure DbContexts - These are needed by the Gateway to resolve dependencies for MediatR handlers
// if the handlers themselves are registered here and need DbContext.
// Alternatively, the DbContexts could be registered in each microservice's API project and injected from there.
// For a single deployable unit (Option B), registering them here is appropriate.
builder.Services.AddDbContext<FormarkaLMS.Services.Courses.Infrastructure.Persistence.CoursesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<FormarkaLMS.Services.Identity.Infrastructure.Persistence.IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<FormarkaLMS.Services.Learning.Infrastructure.Persistence.LearningDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories and MediatR handlers for each microservice
// It's important that the MediatR registration scans the correct Application assemblies.

// Courses Service Application Layer & Domain Interfaces
builder.Services.AddScoped<FormarkaLMS.Services.Courses.Domain.Interfaces.ICourseRepository, FormarkaLMS.Services.Courses.Infrastructure.Repositories.CourseRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(FormarkaLMS.Services.Courses.Application.DTOs.CourseDto).Assembly));

// Identity Service Application Layer & Domain Interfaces
builder.Services.AddScoped<FormarkaLMS.Services.Identity.Domain.Interfaces.IUserProfileRepository, FormarkaLMS.Services.Identity.Infrastructure.Repositories.UserProfileRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(FormarkaLMS.Services.Identity.Application.Users.Queries.GetUserProfileByIdQuery).Assembly));

// Learning Service Application Layer & Domain Interfaces
builder.Services.AddScoped<FormarkaLMS.Services.Learning.Domain.Interfaces.IEnrollmentRepository, FormarkaLMS.Services.Learning.Infrastructure.Repositories.EnrollmentRepository>();
builder.Services.AddScoped<FormarkaLMS.Services.Learning.Domain.Interfaces.ILessonProgressRepository, FormarkaLMS.Services.Learning.Infrastructure.Repositories.LessonProgressRepository>();
builder.Services.AddScoped<FormarkaLMS.Services.Learning.Domain.Interfaces.IQuizRepository, FormarkaLMS.Services.Learning.Infrastructure.Repositories.QuizRepository>();
builder.Services.AddScoped<FormarkaLMS.Services.Learning.Domain.Interfaces.IQuizResultRepository, FormarkaLMS.Services.Learning.Infrastructure.Repositories.QuizResultRepository>();
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
