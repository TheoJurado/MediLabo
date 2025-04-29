using Microsoft.EntityFrameworkCore;
using MediLabo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MediLabo.Data
{
    public class ApplicationDbContext : DbContext//IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Patient> Patient { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Note> Notes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation Patient -> Note (0..1 : 1..1)
            modelBuilder.Entity<Note>()
                .HasOne(n => n.Patient)
                .WithMany(p => p.Notes)
                .HasForeignKey(n => n.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
