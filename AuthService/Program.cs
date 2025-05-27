using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            //add SQL
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddIdentity<Doctor, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            #region JWT part
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
                var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
            #endregion

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    int maxRetries = 5;
                    TimeSpan delayBetweenRetries = TimeSpan.FromSeconds(10);

                    for (int attempt = 1; attempt <= maxRetries; attempt++)
                    {
                        try
                        {
                            logger.LogInformation($"Tentative {attempt}/{maxRetries} d'initialisation de la base de données (application des migrations)...");

                            await db.Database.MigrateAsync();
                            logger.LogInformation("Migrations appliquées avec succès.");

                            logger.LogInformation("Démarrage du seeding des données...");
                            await SeedData.InitializeSQL(scope.ServiceProvider);
                            logger.LogInformation("Seeding des données terminé.");

                            break;
                        }
                        catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 1801)
                        {
                            logger.LogWarning(sqlEx, $"Erreur SQL (Database already exists - {sqlEx.Number}) lors de la tentative {attempt}/{maxRetries}. Cela peut indiquer que la vérification d'existence par EF Core a échoué mais la base est là. Nous allons réessayer.");
                            if (attempt == maxRetries)
                            {
                                logger.LogError("Nombre maximal de tentatives atteint. L'initialisation de la base de données a échoué à cause de l'erreur 'Database already exists' persistante.");
                                throw;
                            }
                            await Task.Delay(delayBetweenRetries);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, $"Erreur lors de l'initialisation de la base de données (Tentative {attempt}/{maxRetries}).");
                            if (attempt == maxRetries)
                            {
                                logger.LogError("Nombre maximal de tentatives atteint. L'initialisation de la base de données a échoué.");
                                throw;
                            }
                            logger.LogInformation($"Nouvelle tentative dans {delayBetweenRetries.TotalSeconds} secondes...");
                            await Task.Delay(delayBetweenRetries);
                        }
                    }
                }

                app.UseSwagger();//
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
