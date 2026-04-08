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
using System.Text;
using Xunit;


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
            var result = new QuestionsController(context).GetQuestions() as OkObjectResult;

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
            var result = new QuestionsController(context).GetQuestions() as OkObjectResult;
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
            var controller = new QuestionsController(context);

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
    }
}
