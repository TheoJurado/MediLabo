using Xunit;
using System;
using System.Collections.Generic;
using Risk.Controllers;
using System.Net;
using Moq;
using Moq.Protected;

namespace MediLaboTest
{
    public class RiskTest
    {
        private readonly RiskController _controller;
        private readonly Mock<IHttpClientFactory> _mockFactory;

        public RiskTest()
        {
            // 1.Stub HttpMessageHandler
            var fakeHandler = new Mock<HttpMessageHandler>();
            fakeHandler.Protected()
              // When SendAsync is call…
              .Setup<Task<HttpResponseMessage>>(
                 "SendAsync",
                 ItExpr.IsAny<HttpRequestMessage>(),
                 ItExpr.IsAny<CancellationToken>()
              )
              // …return 200 OK with minimal JSON or empty
              .ReturnsAsync(new HttpResponseMessage
              {
                  StatusCode = HttpStatusCode.OK,
                  Content = new StringContent("{ \"birthDay\": \"1980-01-01\", \"genderText\": \"Man\" }")
              });

            // 2. Creat HttpClient from this handler
            var client = new HttpClient(fakeHandler.Object)
            {
                BaseAddress = new Uri("http://localhost") // osef / idc
            };

            // 3. Mock IHttpClientFactory for send back OUR HttpClient
            _mockFactory = new Mock<IHttpClientFactory>();
            _mockFactory
                .Setup(f => f.CreateClient("GatewayClient"))
                .Returns(client);

            
            _controller = new RiskController(_mockFactory.Object);
        }

        [Fact]
        public void CountTriggerInNote_CountsDistinctKeywords()
        {
            // Arrange
            var notes = new List<string>
            {
                "Patient a un antécédent de fumeur",
                "Cholestérol en hausse",
                "Le patient est bonne santé"
            };
            // Act
            var count = _controller.CountTriggerInNote(notes);
            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public void CountTriggerInNote_DontCountMultipleTimeSameKeywords()
        {
            // Arrange
            var notes = new List<string>
            {
                "Patient a pris du poids",//poids
                "Le poids du patient est revenue a la normale"//poids
            };
            // Act
            var count = _controller.CountTriggerInNote(notes);
            // Assert
            Assert.Equal(1, count);
        }
    }
}