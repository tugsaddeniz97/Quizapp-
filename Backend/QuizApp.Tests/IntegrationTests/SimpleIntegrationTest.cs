using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Controllers;
using QuizApp.Data;
using QuizApp.Models;
using Xunit;

namespace QuizApp.Tests.IntegrationTests
{
    public class SimpleIntegrationTest
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void Integration_PlayerAndQuestionFlow_WorksTogether()
        {
            // Arrange - Setup database
            var context = GetDbContext();

            var player = new Player
            {
                Username = "TestPlayer",
                TotalScore = 0
            };
            context.Players.Add(player);

            var question = new Question
            {
                Type = "multiple",
                Difficulty = "easy",
                Category = "Test",
                Text = "What is 2+2?",
                CorrectAnswer = "4",
                IncorrectAnswersJson = "[\"3\",\"5\",\"6\"]"
            };
            context.Questions.Add(question);
            context.SaveChanges();

            // Act - Test integration between Player and Game controllers
            var playerController = new PlayerController(context);
            var gameController = new GameController(context);

            var playersResult = playerController.GetPlayers() as OkObjectResult;

            // Assert
            Assert.NotNull(playersResult);
            var players = playersResult.Value as List<Player>;
            Assert.Single(players);
            Assert.Equal("TestPlayer", players[0].Username);
        }

        [Fact]
        public void Integration_CompleteGameFlow_PlayerAnswersQuestion()
        {
            // Arrange
            var context = GetDbContext();

            var player = new Player { Username = "Gamer", TotalScore = 0 };
            var question = new Question
            {
                Type = "multiple",
                Difficulty = "easy",
                Category = "Math",
                Text = "What is 5+5?",
                CorrectAnswer = "10",
                IncorrectAnswersJson = "[\"8\",\"9\",\"11\"]"
            };

            context.Players.Add(player);
            context.Questions.Add(question);
            context.SaveChanges();

            // Act - Simulate game flow
            var gameController = new GameController(context);
            var playRequest = new GameController.PlayRequest
            {
                PlayerId = player.PlayerId,
                QuestionId = question.Id,
                Answer = "10",
                TimeTakenSeconds = 3
            };

            var result = gameController.Play(playRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var response = result.Value as QuizApp.DTOs.PlayResponse;
            Assert.True(response.Correct);
            Assert.True(response.PointsEarned > 0);
        }
    }
}