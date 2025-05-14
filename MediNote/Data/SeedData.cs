using MediNote.Models;
using MongoDB.Driver;

namespace MediNote.Data
{
    public class SeedData
    {
        public static void InitializeMongoNote(IMongoDatabase database)
        {
            Console.WriteLine("SeedData: Démarrage de l'initialisation des notes");
            var NoteCollection = database.GetCollection<Note>("Notes");
            Console.WriteLine("notes colectées");
            var patientCollection = database.GetCollection<Patient>("Patients");
            Console.WriteLine("patients colectés");
            //return;

            if (patientCollection.Find(_ => true).Any())
            {
                Console.WriteLine("analyse des patients");
                //recover all default Patients
                var testNone = patientCollection.Find(p => p.Name == "TestNone").FirstOrDefault();
                var testBorderline = patientCollection.Find(p => p.Name == "TestBorderline").FirstOrDefault();
                var testInDanger = patientCollection.Find(p => p.Name == "TestInDanger").FirstOrDefault();
                var testEarlyOnset = patientCollection.Find(p => p.Name == "TestEarlyOnset").FirstOrDefault();

                Console.WriteLine("verification des patients");
                // Check if patients exist
                if (testNone == null || testBorderline == null || testInDanger == null || testEarlyOnset == null)
                {
                    Console.WriteLine("Un ou plusieurs patients nécessaires à l'initialisation des notes sont introuvables.");
                    return;
                }

                Console.WriteLine("recherche des notes");
                if (!NoteCollection.Find(_ => true).Any())
                {
                    Console.WriteLine("Note absente > création des notes");
                    //Adding default notes
                    var notes = new List<Note>
                    {
                    new Note
                    {
                        Comment = "Le patient déclare qu'il 'se sent très bien' Poids égal ou inférieur au poids recommandé",
                        PatientId = testNone.Id
                    },
                    new Note
                    {
                        Comment = "Le patient déclare qu'il ressent beaucoup de stress au travail Il se plaint également que son audition est anormale dernièrement",
                        PatientId = testBorderline.Id
                    },
                    new Note
                    {
                        Comment = "Le patient déclare avoir fait une réaction aux médicaments au cours des 3 derniers mois Il remarque également que son audition continue d'être anormale",
                        PatientId = testInDanger.Id
                    },
                    new Note
                    {
                        Comment = "Le patient déclare qu'il fume depuis peu",
                        PatientId = testInDanger.Id
                    },
                    new Note
                    {
                        Comment = "Le patient déclare qu'il est fumeur et qu'il a cessé de fumer l'année dernière Il se plaint également de crises d’apnée respiratoire anormales Tests de laboratoire indiquant un taux de cholestérol LDL élevé",
                        PatientId = testInDanger.Id
                    },
                    new Note
                    {
                        Comment = "Le patient déclare qu'il lui est devenu difficile de monter les escaliers Il se plaint également d’être essoufflé Tests de laboratoire indiquant que les anticorps sont élevés Réaction aux médicaments",
                        PatientId = testEarlyOnset.Id
                    },
                    new Note
                    {
                        Comment = "Le patient déclare qu'il a mal au dos lorsqu'il reste assis pendant longtemps",
                        PatientId = testEarlyOnset.Id
                    },
                    new Note
                    {
                        Comment = " Le patient déclare avoir commencé à fumer depuis peu Hémoglobine A1C supérieure au niveau recommandé",
                        PatientId = testEarlyOnset.Id
                    },
                    new Note
                    {
                        Comment = " Taille, Poids, Cholestérol, Vertige et Réaction",
                        PatientId = testEarlyOnset.Id
                    }
                    };
                    NoteCollection.InsertMany(notes);
                }
            }

            Console.WriteLine("SeedData: fin de l'initialisation des notes");
        }
    }
}
