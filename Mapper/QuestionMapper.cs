using QuizApp.DTOs;
using QuizApp.Models;
using System.Text.Json;

namespace QuizApp.Mapper
{
    public class QuestionMapper
    {
        public static Question ToEntity(QuestionDTO dto)
        {
            return new Question
            {
                Type = dto.Type,
                Difficulty = dto.Difficulty,
                Category = dto.Category,
                Text = dto.Question,
                CorrectAnswer = dto.Correct_Answer,
                IncorrectAnswersJson = JsonSerializer.Serialize(dto.Incorrect_Answers)
            };
        }

        public static QuestionDTO ToDTO(Question entity)
        {
            return new QuestionDTO
            {
                Type = entity.Type,
                Difficulty = entity.Difficulty,
                Category = entity.Category,
                Question = entity.Text,
                Correct_Answer = entity.CorrectAnswer,
                Incorrect_Answers = JsonSerializer.Deserialize<List<string>>(entity.IncorrectAnswersJson)
            };
        }
    }
}
