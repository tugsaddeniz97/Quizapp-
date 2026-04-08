using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using QuizApp.DTOs;
using Xunit;

namespace QuizApp.Tests.IntegrationTests
{
    public class SimpleIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public SimpleIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetPlayers_ReturnsOkStatus()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Player");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        
    }
}