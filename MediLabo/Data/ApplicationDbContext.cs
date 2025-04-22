using Microsoft.EntityFrameworkCore;
using MediLabo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MediLabo.Data
{
    public class ApplicationDbContext : DbContext//IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Patient> Utilisateurs { get; set; }
    }
}
