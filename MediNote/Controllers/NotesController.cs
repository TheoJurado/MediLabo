using Microsoft.AspNetCore.Mvc;
using MediNote.Models;
using System.Net.Http;

namespace MediNote.Controllers
{
    [ApiController]
    [Route("noteapi/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public NotesController(INoteRepository noteRepository, IHttpClientFactory httpClientFactory)
        {
            _noteRepository = noteRepository;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes()
        {
            var patients = await _noteRepository.GetAllNotesAsync();
            return Ok(patients);
        }

        [HttpGet("{patientId}/notes")]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotesFromThisPatient(string patientId)
        {
            var notes = await _noteRepository.GetAllNotesFromPatientByHisId(patientId);
            return Ok(notes);
        }

        [HttpPost("{patientId}/note")]
        public async Task<ActionResult> AddNoteToPatient(string patientId, [FromBody] string note)
        {
            Console.WriteLine("demande de note : " +  note);
            var httpClient = _httpClientFactory.CreateClient("MediLabo");
            var response = await httpClient.GetAsync($"/api/patients/{patientId}");
            if (!response.IsSuccessStatusCode)
                return NotFound($"Patient with ID {patientId} not found.");

            _noteRepository.AddNoteToPatient(CreatNoteFromString(note, patientId), patientId);
            return Ok();
        }

        [HttpDelete("deletenotes/{noteId}")]
        public async Task<ActionResult> DeleteNote(string noteId)
        {
            var existingNote = await _noteRepository.GetNoteById(noteId);
            if (existingNote == null)
            {
                return NotFound($"Note with ID {noteId} not found.");
            }

            _noteRepository.DeleteNote(existingNote);
            return NoContent();
        }

        private Note CreatNoteFromString(string note, string patientId)
        {
            Note newNote = new Note();
            newNote.Comment = note;
            newNote.PatientId = patientId;
            newNote.Date = DateOnly.FromDateTime(DateTime.Today);

            return newNote;
        }

    }
}
