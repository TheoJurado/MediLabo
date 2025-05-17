using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Risk.Data;
using System.Text.Json;

namespace Risk.Controllers
{
    [ApiController]
    [Route("riskapi/[controller]")]
    public class RiskController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public RiskController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("GatewayClient");
        }

        [HttpGet("{idPatient}/riskpatient")]
        public async Task<string> GetRiskForThisPatientAsync(string idPatient)
        {
            //patient part
            var patientResponse = await _httpClient.GetAsync($"/medilabo/patients/{idPatient}");//get patient infos
            if (!patientResponse.IsSuccessStatusCode)
                return "Patient non trouvé";
            var patientContent = await patientResponse.Content.ReadAsStringAsync();//translate infos p1
            var patient = JsonSerializer.Deserialize<PatientDto>(patientContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });//translate p2
            if (patient == null)
                return "Patient inconnu";

            //note part
            var medinoteResponse = await _httpClient.GetAsync($"/medinote/notes/{idPatient}/notes");//get note infos
            if (!medinoteResponse.IsSuccessStatusCode)
                return "Notes non trouvées";
            var noteContent = await medinoteResponse.Content.ReadAsStringAsync();//translate p1
            var notes = JsonSerializer.Deserialize<List<NoteDto>>(noteContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });//translate p2
            if (notes == null)
                return "Note inconnu";
            List<string> notesString = notes.Select(n => n.Comment).ToList();//get a list of comment without idPatent

            //return part
            return RiskResult(CountTriggerInNote(notesString), AgeCalculator(patient.BirthDay), patient.GenderText);
        }

        public string RiskResult(int nbTrigger, int agePatient, string genrePatient)
        {
            string resultNone = "Aucun risque (None)";
            string resultBorderline = "Risque limité (Borderline)";
            string resultInDanger = "Danger (In Danger)";
            string resultEarlyOnset = "Apparition précoce (Early onset)";

            if (agePatient >= 30)
            {//30 years is in the 30+ categorie
                if (nbTrigger >= 8)
                    return resultEarlyOnset;
                if (nbTrigger >= 6)
                    return resultInDanger;
                if (nbTrigger >= 2)
                    return resultBorderline;
                return resultNone;
            }
            else
            {//30 years is NOT in 30- categorie
                if (genrePatient == "Man")
                {
                    if (nbTrigger >= 5)
                        return resultEarlyOnset;
                    if (nbTrigger >= 3)
                        return resultInDanger;
                    return resultNone;
                }
                if (genrePatient == "Woman")
                {
                    if (nbTrigger >= 7)
                        return resultEarlyOnset;
                    if (nbTrigger >= 4)
                        return resultInDanger;
                    return resultNone;
                }
                return "Demandez à votre médecin";
            }
        }

        public int CountTriggerInNote(List<string> notes)
        {
            // Initialize a dictionary where each keyword in triggerSearch is a key, initialized to false (not found)
            Dictionary<string, bool> resultDictionary = new Dictionary<string, bool>();
            foreach (string trigger in Trigger.allTriggers)
            {
                resultDictionary[trigger] = false;
            }
            
            foreach (string note in notes)
            {
                foreach (string trigger in Trigger.allTriggers)
                {//foreach note, check if any trigger is in
                    if (note.Contains(trigger, StringComparison.OrdinalIgnoreCase)) //HELLO = hello
                    {
                        resultDictionary[trigger] = true;
                    }
                }
            }
            //return how many different trigger was found
            return resultDictionary.Values.Count(v => v == true);
        }

        public int AgeCalculator(DateTime BirthDay)
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDay.Year;
            // If the birthday has not yet passed this year, we subtract 1
            if (BirthDay.Date > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }
}
