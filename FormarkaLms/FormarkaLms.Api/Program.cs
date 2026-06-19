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
    // Creamos una única política que permite ambos entornos
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:4200",                                         // Desarrollo local
                    "https://victorious-pond-0fbb6b710.7.azurestaticapps.net"        // Producción en Azure
                  )
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
            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role // Aseguramos que busque aquí
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
                
                var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                if (claimsIdentity == null) return Task.CompletedTask;

                string? role = null;

                // Intentar extraer de app_metadata (preferido para roles de sistema) o user_metadata
                if (context.SecurityToken is Microsoft.IdentityModel.JsonWebTokens.JsonWebToken jsonToken)
                {
                    // Buscar en app_metadata
                    if (jsonToken.TryGetPayloadValue<System.Text.Json.JsonElement>("app_metadata", out var appMetadata) && 
                        appMetadata.TryGetProperty("role", out var appRole))
                    {
                        role = appRole.GetString();
                    }
                    
                    // Si no está, buscar en user_metadata
                    if (string.IsNullOrEmpty(role) && 
                        jsonToken.TryGetPayloadValue<System.Text.Json.JsonElement>("user_metadata", out var userMetadata) && 
                        userMetadata.TryGetProperty("role", out var userRole))
                    {
                        role = userRole.GetString();
                    }
                }
                else if (context.SecurityToken is System.IdentityModel.Tokens.Jwt.JwtSecurityToken jwtToken)
                {
                    // Lógica para JwtSecurityToken (respaldo)
                    var claims = jwtToken.Claims.ToList();
                    
                    var appMeta = claims.FirstOrDefault(c => c.Type == "app_metadata")?.Value;
                    if (!string.IsNullOrEmpty(appMeta)) {
                        try {
                            var doc = System.Text.Json.JsonDocument.Parse(appMeta);
                            if (doc.RootElement.TryGetProperty("role", out var r)) role = r.GetString();
                        } catch {}
                    }

                    if (string.IsNullOrEmpty(role)) {
                        var userMeta = claims.FirstOrDefault(c => c.Type == "user_metadata")?.Value;
                        if (!string.IsNullOrEmpty(userMeta)) {
                            try {
                                var doc = System.Text.Json.JsonDocument.Parse(userMeta);
                                if (doc.RootElement.TryGetProperty("role", out var r)) role = r.GetString();
                            } catch {}
                        }
                    }
                }

                if (!string.IsNullOrEmpty(role))
                {
                    var normalizedRole = char.ToUpper(role[0]) + role.Substring(1).ToLower();
                    Console.WriteLine($"--- ROL IDENTIFICADO: {normalizedRole} ---");
                    
                    // Agregamos el rol. Usamos el RoleClaimType configurado en Identity
                    claimsIdentity.AddClaim(new Claim(claimsIdentity.RoleClaimType, normalizedRole));
                }
                else 
                {
                    Console.WriteLine("--- ADVERTENCIA: No se encontró un rol en los metadatos del token ---");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// CORS MUST be at the very top to handle preflight requests
app.UseCors("AllowFrontend");

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
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
