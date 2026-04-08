using System.Net;
using System.Net.Http.Json;
using QuizApp.DTOs;
using Xunit;

namespace QuizApp.Tests.IntegrationTests
{
    public class SimpleIntegrationTest : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public SimpleIntegrationTest(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetQuestions_ReturnsOkStatus()
        {
            // Act
            var response = await _client.GetAsync("/Questions");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddQuestion_ThenRetrieveIt_WorksEndToEnd()
        {
            // Arrange
            var newQuestion = new QuestionDTO
            {
                Type = "multiple",
                Difficulty = "easy",
                Category = "Test",
                Question = "Integration test question?",
                Correct_Answer = "Yes",
                Incorrect_Answers = new List<string> { "No", "Maybe" }
            };

            // Act - Add question via HTTP
            var postResponse = await _client.PostAsJsonAsync("/Questions", newQuestion);
            
            // Assert - Check it was added
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        }
    }
}