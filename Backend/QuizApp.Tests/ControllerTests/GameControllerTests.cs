using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Controllers;
using QuizApp.Data;
using QuizApp.DTOs;
using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Tests.ControllerTests
{
    
    public class GameControllerTests
    {
        [Fact]
        public void GameController_Play_ReturnsCorrectWhenAnswerIsRight()
        {
            // Arrange
            var context = GetDbContext();
            var player = CreatePlayer(context);
            var question = CreateQuestion(context);

            var controller = new GameController(context);
            var request = new GameController.PlayRequest
            {
                PlayerId = player.PlayerId,
                QuestionId = question.Id,
                Answer = question.CorrectAnswer,
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

        [Fact]
        public void GameController_Play_ReturnsWrongWhenAnswerIsWrong()
        {
            //Arrange
            var context = GetDbContext();
            var player = CreatePlayer(context);
            var question = CreateQuestion(context);


            var controller = new GameController(context);
            var request = new GameController.PlayRequest
            {
                PlayerId = player.PlayerId,
                QuestionId = question.Id,
                Answer = "London",
                TimeTakenSeconds = 10
            };

            //Act
            var result = controller.Play(request) as OkObjectResult;


            //Assert
            Assert.NotNull(result);
            var response = result.Value as PlayResponse;
            Assert.NotNull(response);
            Assert.False(response.Correct);
            Assert.Equal(0, response.PointsEarned);
            Assert.Equal(0, response.TotalScore);

        }

        [Fact]
        public void GameController_Play_ReturnsBadRequestWhenPlayerNotFound()
        {
            //Arrange
            var context = GetDbContext();
            var controller = new GameController(context);
            var question = CreateQuestion(context);

            var request = new GameController.PlayRequest
            {
                PlayerId = 1,
                QuestionId = question.Id,
                Answer = question.CorrectAnswer,
                TimeTakenSeconds = 10
            };
            //Act
            var result = controller.Play(request) as BadRequestObjectResult;

            //Assert

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GameController_Play_Returns10PointsWhenAnswerIsCorrectAndTimeIsMoreThan5Seconds()
        {
            //Arrange
            var context = GetDbContext();
            var player = CreatePlayer(context);
            var question = CreateQuestion(context);


            var controller = new GameController(context);
            var request = new GameController.PlayRequest
            {
                PlayerId = player.PlayerId,
                QuestionId = question.Id,
                Answer = question.CorrectAnswer,
                TimeTakenSeconds = 6
            };

            //Act
            var result = controller.Play(request) as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            var response = result.Value as PlayResponse;
            Assert.NotNull(response);
            Assert.True(response.Correct);
            Assert.Equal(10, response.PointsEarned);

        }

        [Fact]
        public void GameController_Play_Returns13PointsWhenAnswerIsCorrectAndTimeIs2Seconds()
        {
            //Arrange
            var context = GetDbContext();
            var player = CreatePlayer(context);
            var question = CreateQuestion(context);


            var controller = new GameController(context);

            var request = new GameController.PlayRequest
            {
                PlayerId = player.PlayerId,
                QuestionId = question.Id,
                Answer = question.CorrectAnswer,
                TimeTakenSeconds = 2
            };
            //Act
            var result = controller.Play(request) as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            var response = result.Value as PlayResponse;
            Assert.NotNull(response);
            Assert.True(response.Correct);
            Assert.Equal(13, response.PointsEarned);


        }

        //Helper methods
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

            return new AppDbContext(options);
        }

        private Player CreatePlayer(AppDbContext context)
        {
            var player = new Player
            {
                Username = "TestUser",
                TotalScore = 0
            };
            context.Players.Add(player);
            context.SaveChanges();
            return player;
        }

        private Question CreateQuestion(AppDbContext context)
        {
            var question = new Question
            {
                Type = "multiple",
                Difficulty = "medium",
                Category = "Maths",
                Text = "what is the capital city of France",
                CorrectAnswer = "Paris",
                IncorrectAnswersJson = "Something"
            };
            context.Questions.Add(question);
            context.SaveChanges();
            return question;
        }
    }
}
