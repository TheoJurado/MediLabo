using MediNote.Controllers;
using MediNote.Models;
using Microsoft.AspNetCore.Mvc;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MediLaboTest
{
    public class NoteTest
    {
        private readonly NotesController _controller;
        private readonly Mock<INoteRepository> _mockNoteRepository;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

        public NoteTest()
        {
            // 1. Mock repository
            _mockNoteRepository = new Mock<INoteRepository>();

            // 2. Mock HttpClientFactory
            var fakeHandler = new Mock<HttpMessageHandler>();
            fakeHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{}")
                });

            var client = new HttpClient(fakeHandler.Object);
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(client);

            // 3. Creat contrôleur with mocks dependances
            _controller = new NotesController(_mockNoteRepository.Object, _mockHttpClientFactory.Object);
        }

        [Fact]
        public async Task GetAllNotesFromThisPatient_ReturnsOkResult_WithListOfNotes()
        {
            // Arrange
            var patientId = "123";
            var expectedNotes = new List<Note>
            {
                new Note { Id = "1", PatientId = "123", Comment = "Note 1" },
                new Note { Id = "2", PatientId = "123", Comment = "Note 2" }
            };

            _mockNoteRepository
                .Setup(repo => repo.GetAllNotesFromPatientByHisId(patientId))
                .ReturnsAsync(expectedNotes);

            // Act
            var result = await _controller.GetAllNotesFromThisPatient(patientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Note>>(okResult.Value);
            Assert.Equal(2, ((List<Note>)returnValue).Count);
        }
    }
}
