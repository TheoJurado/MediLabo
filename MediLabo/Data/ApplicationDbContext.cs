using Microsoft.EntityFrameworkCore;
using MediLabo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MediLabo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Doctor> Doctors { get; set; }

    }
}
