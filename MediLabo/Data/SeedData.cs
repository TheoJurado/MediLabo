using Microsoft.EntityFrameworkCore;
using MediLabo.Models;

namespace MediLabo.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());


            //return;
            if (context.Patient.Any())
            {
                return;
            }

            context.Patient.AddRange(
                new Patient
                {
                    Name = "TestNone",
                    FirstName = "Test",
                    BirthDay = new DateTime(1966, 12, 31),
                    Gender = GenderType.Woman,
                    Adress = "1 Brookside St",
                    Phone = "100-222-3333"
                },
                new Patient
                {
                    Name = "TestBorderline",
                    FirstName = "Test",
                    BirthDay = new DateTime(1945, 06, 24),
                    Gender = GenderType.Man,
                    Adress = "2 High St",
                    Phone = "200-333-4444"
                },
                new Patient
                {
                    Name = "TestInDanger",
                    FirstName = "Test",
                    BirthDay = new DateTime(2004, 06, 18),
                    Gender = GenderType.Man,
                    Adress = "3 Club Road",
                    Phone = "300-444-5555"
                },
                new Patient
                {
                    Name = "TestEarlyOnset",
                    FirstName = "Test",
                    BirthDay = new DateTime(2002, 06, 28),
                    Gender = GenderType.Woman,
                    Adress = "4 Valley Dr",
                    Phone = "400-555-6666"
                }
            );
            context.SaveChanges();/**/
        }
    }
}
