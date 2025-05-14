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

        [HttpGet("{id}/notes")]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotesFromThisPatient(string patientId)
        {
            var notes = await _noteRepository.GetAllNotesFromPatientByHisId(patientId);
            return Ok(notes);
        }

        [HttpPost("{patientId}/notes")]
        public async Task<ActionResult> AddNoteToPatient(string patientId, [FromBody] Note note)
        {
            var httpClient = _httpClientFactory.CreateClient("MediLabo");
            var response = await httpClient.GetAsync($"/api/patients/{patientId}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound($"Patient with ID {patientId} not found.");
            }

            _noteRepository.AddNoteToPatient(note, patientId);
            return Ok();
        }

        [HttpDelete("notes/{noteId}")]
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
    }
}
