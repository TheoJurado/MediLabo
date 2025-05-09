using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Numerics;
using AuthService.Models;

namespace AuthService.Data
{
    public class SeedData
    {
        public static async Task InitializeSQL(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            var doctorManager = serviceProvider.GetRequiredService<UserManager<Doctor>>();

            if (!context.Doctors.Any())
            {
                var doc = new Doctor
                {
                    UserName = "Doc1",
                    Email = "doc1@example.com",
                    EmailConfirmed = true
                };
                var result = await doctorManager.CreateAsync(doc, "Doc1PassW0rd!");

                if (!result.Succeeded)
                {
                    // print errors
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating doctor: {error.Description}");
                    }
                }
            }
        }
    }
}
