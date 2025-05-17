using MediLabo.Models;
using MongoDB.Driver;

namespace MediLabo.Data
{
    public class SeedData
    {
        public static void InitializeMongoPatient(IMongoDatabase database)
        {
            Console.WriteLine("SeedData: Démarrage de l'initialisation des patients");
            var patientCollection = database.GetCollection<Patient>("Patients");

            if (!patientCollection.Find(_ => true).Any())
            {
                var patients = new List<Patient>
                {
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
                };
                patientCollection.InsertMany(patients);
            }

            Console.WriteLine("SeedData: fin de l'initialisation des patients");
        }

    }
}
