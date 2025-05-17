using MediLabo.Data;
using MongoDB.Driver;
using MediLabo.Models;

namespace MediLabo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            // add MongoDB
            builder.Services.Configure<MongoDbSettings>(
                builder.Configuration.GetSection("MongoDbSettings"));
            builder.Services.AddSingleton<IMongoClient>(s =>
                new MongoClient(builder.Configuration.GetSection("MongoDbSettings")["ConnectionString"]));
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    //MongoDB part
                    var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
                    var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
                    var database = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);

                    SeedData.InitializeMongoPatient(database);//Mongo
                }
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
