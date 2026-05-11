using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FormarkaLMS.Shared.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine($"[EF Core] Directorio de ejecución: {currentDirectory}");

            var relativePathToApi = "../../Gateway/FormarkaLMS.Gateway";
            var apiProjectPath = Path.GetFullPath(Path.Combine(currentDirectory, relativePathToApi));

            Console.WriteLine($"[EF Core] Buscando appsettings en: {apiProjectPath}");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return new ApplicationDbContext(optionsBuilder.Options, configuration);
        }
    }
}
