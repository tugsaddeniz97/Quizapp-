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

        public static PlayQuestionDTO ToPlayDTO(Question question)
        {
            var incorrectAnswers = JsonSerializer.Deserialize<List<String>>(question.IncorrectAnswersJson);
            var allAnswers = new List<string>();

            if (incorrectAnswers != null)
                allAnswers.AddRange(incorrectAnswers);
            allAnswers.Add(question.CorrectAnswer);


            var rnd = new Random();
            allAnswers = allAnswers.OrderBy(x => rnd.Next()).ToList();

            return new PlayQuestionDTO
            {
                Id = question.Id,
                Question = question.Text,
                Answers = allAnswers
            };

        }
    }
}
