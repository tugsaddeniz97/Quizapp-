using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Tests.ControllerTests
{
    
    public class GameControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            return new AppDbContext(options);
        }


        [Fact]
        private void GameController_Play_ReturnsCorrectWhenAnswerIsRight()
        {
            // Arrange
            var context = GetDbContext();
            var player = new Models.Player { Username = "TestPlayer", TotalScore = 0 };
            var question = new Models.Question { Id=2, Type= "multiple", Difficulty= "medium", Category="Maths", CorrectAnswer= "Hanoi", IncorrectAnswersJson="test", 
            Text="what is 2+2"};


            context.Players.Add(player);
            context.Questions.Add(question);
            context.SaveChanges();


            var controller = new Controllers.GameController(context);
            var request = new Controllers.GameController.PlayRequest
            {
                PlayerId = player.PlayerId,
                QuestionId = question.Id,
                Answer = "Hanoi",
                TimeTakenSeconds = 10
            };


            //Act
            var result = controller.Play(request) as OkObjectResult;
            Assert.NotNull(result);

            // Assert
            var response = result.Value as PlayResponse;
           
            Assert.NotNull(response);
            Assert.True(response.Correct);
            Assert.True(response.PointsEarned > 0);
            Assert.True(response.TotalScore >= 0);
        }
        

    }
}
