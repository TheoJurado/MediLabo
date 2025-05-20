using MediNote.Data;
using MediNote.Models;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;

namespace MediNote
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddHttpClient("MediLabo", client =>
            {
                client.BaseAddress = new Uri("http://gateway.ocelot:8080");
            });

            // add MongoDB
            builder.Services.Configure<MongoDbSettings>(
                builder.Configuration.GetSection("MongoDbSettings"));
            builder.Services.AddSingleton<IMongoClient>(s =>
                new MongoClient(builder.Configuration.GetSection("MongoDbSettings")["ConnectionString"]));
            builder.Services.AddScoped<INoteRepository, NoteRepository>();


            //add authentification
            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://authservice:8080";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

                    SeedData.InitializeMongoNote(database);//Mongo
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
