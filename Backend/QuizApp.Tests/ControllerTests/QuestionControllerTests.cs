using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Controllers;
using QuizApp.Data;
using QuizApp.DTOs;
using QuizApp.Mapper;
using QuizApp.Models;
using QuizApp.Mapper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Moq;
using Moq.Protected;


namespace QuizApp.Tests.ControllerTests
{
    public class QuestionControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void QuestionController_GetQuestions_ReturnsEmptyListWhenNoQuestions()
        {
            //Arrange
            var context = GetDbContext();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var result = new QuestionsController(context, mockHttpClientFactory.Object).GetQuestions() as OkObjectResult;

            //Act
            Assert.NotNull(result);
            var quetions = result.Value as IEnumerable<QuestionDTO>;

            //Assert

            Assert.NotNull(quetions);
            Assert.Empty(quetions);
        }

        [Fact]
        public void QuestionController_GetQuestions_ReturnsListOfQuestions()
        {
            //Arrange
            var context = GetDbContext();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var question1 = new Question
            {
                Type = "multiple",
                Difficulty = "medium",
                Category = "Geography",
                Text = "what is the capital city of France",
                CorrectAnswer = "Paris",
                IncorrectAnswersJson = "[\"London\", \"Berlin\", \"Madrid\"]"
            };

            var question2 = new Question
            {
                Type = "multiple",
                Difficulty = "medium",
                Category = "Maths",
                Text = "what is 2 + 2",
                CorrectAnswer = "Paris",
                IncorrectAnswersJson = "[\"1\", \"3\", \"5\"]"
            };

            context.Questions.AddRange(question1, question2);
            context.SaveChanges();


            var expectedQuestionsList = new List<QuestionDTO> { QuestionMapper.ToDTO(question1), QuestionMapper.ToDTO(question2) };
            //Act
            var result = new QuestionsController(context, mockHttpClientFactory.Object).GetQuestions() as OkObjectResult;
            Assert.NotNull(result);

            var questionsList = result.Value as IEnumerable<QuestionDTO>;

            Assert.NotNull(questionsList);

            Assert.Equal(expectedQuestionsList.Count, questionsList.Count());
            Assert.Equal(expectedQuestionsList, questionsList);
            //Assert

        }

        [Fact]
        public void QuestionController_AddQuestion_AddsQuestionToDatabase()
        {
            //Arrange
            var context = GetDbContext();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var controller = new QuestionsController(context, mockHttpClientFactory.Object);

            var questionDTO = new QuestionDTO()
            {
                Type = "multiple",
                Difficulty = "medium",
                Category = "Maths",
                Question = "what is 2 + 2",
                Correct_Answer = "4",
                Incorrect_Answers = new List<string> { "1", "3", "5" }
            };


            //Act

            var result = controller.AddQuestion(questionDTO) as OkObjectResult;

            //Assert

            Assert.NotNull(result);
            var questionInDb = context.Questions.ToList();
            Assert.Single(questionInDb);
            var addedQuestion = questionInDb.First();
            Assert.Equal(questionDTO.Type, addedQuestion.Type);
            Assert.Equal(questionDTO.Difficulty, addedQuestion.Difficulty);
            Assert.Equal(questionDTO.Category, addedQuestion.Category);
            Assert.Equal(questionDTO.Question, addedQuestion.Text);
            Assert.Equal(questionDTO.Correct_Answer, addedQuestion.CorrectAnswer);

            var incorrectAnswers = System.Text.Json.JsonSerializer.Deserialize<List<string>>(addedQuestion.IncorrectAnswersJson);
            Assert.NotNull(incorrectAnswers);
            Assert.Equal(3, incorrectAnswers.Count);
            Assert.Contains("1", incorrectAnswers);
            Assert.Contains("3", incorrectAnswers);
            Assert.Contains("5", incorrectAnswers);

        }

        [Fact]
        public async Task QuestionController_FetchQuestionsFromApi_FetchesAndSavesQuestionsSuccessfully()
        {
            //Arrange
            var context = GetDbContext();
            var mockApiResponse = new QuestionApiResponse
            {
                Response_Code = 0,
                Results = new List<QuestionDTO>
                {
                    new QuestionDTO
                    {
                        Type = "multiple",
                        Difficulty = "easy",
                        Category = "Science",
                        Question = "What is H2O?",
                        Correct_Answer = "Water",
                        Incorrect_Answers = new List<string> { "Oxygen", "Hydrogen", "Carbon" }
                    },
                    new QuestionDTO
                    {
                        Type = "multiple",
                        Difficulty = "medium",
                        Category = "History",
                        Question = "When did World War II end?",
                        Correct_Answer = "1945",
                        Incorrect_Answers = new List<string> { "1944", "1946", "1943" }
                    }
                }
            };

            var jsonResponse = JsonSerializer.Serialize(mockApiResponse);
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var controller = new QuestionsController(context, mockHttpClientFactory.Object);

            //Act
            var result = await controller.FetchQuestionsFromApi(10) as OkObjectResult;

            //Assert
            Assert.NotNull(result);
            var questionsInDb = context.Questions.ToList();
            Assert.Equal(2, questionsInDb.Count);
            Assert.Equal("What is H2O?", questionsInDb[0].Text);
            Assert.Equal("Water", questionsInDb[0].CorrectAnswer);
            Assert.Equal("When did World War II end?", questionsInDb[1].Text);
            Assert.Equal("1945", questionsInDb[1].CorrectAnswer);
        }
    }
}
