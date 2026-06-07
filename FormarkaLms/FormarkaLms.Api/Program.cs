using FormarkaLms.Application.DependencyInjection;
using FormarkaLms.Infrastructure.DependencyInjection;
using FormarkaLms.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add User Secrets in development environment if not already added by default
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 1. IMPORTANTE: Como estamos en Program.cs (Top-level statements), 
// podemos usar 'await' para descargar las llaves directamente de Supabase al arrancar la app.
var supabaseUrl = builder.Configuration["Supabase:ProjectUrl"];

var httpClient = new HttpClient();
var jwksJson = await httpClient.GetStringAsync($"{supabaseUrl}/auth/v1/.well-known/jwks.json");
var supabaseKeys = new JsonWebKeySet(jwksJson).GetSigningKeys();

// 2. Configuramos la autenticación
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = supabaseKeys,

            ValidateIssuer = true,
            ValidIssuers = new[] { $"{supabaseUrl}/auth/v1", supabaseUrl },
            
            ValidateAudience = true,
            ValidAudiences = new[] { "authenticated", "anon" },
            
            ValidateLifetime = true,
            NameClaimType = ClaimTypes.NameIdentifier
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("--- FALLÓ AUTENTICACIÓN ---");
                Console.WriteLine("Error: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("--- TOKEN VALIDADO CORRECTAMENTE ---");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// CORS MUST be at the very top to handle preflight requests
app.UseCors("AllowAngular");

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
