using Microsoft.EntityFrameworkCore;
using MediLabo.Models;

namespace MediLabo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Users> Utilisateurs { get; set; }
    }
}
