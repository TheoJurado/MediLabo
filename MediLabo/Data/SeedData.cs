using Microsoft.EntityFrameworkCore;

namespace MediLabo.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.Utilisateurs.Any())
            {
                return;
            }

            context.Utilisateurs.AddRange(
                new Utilisateurs
                {

                }
                );
        }
    }
}
