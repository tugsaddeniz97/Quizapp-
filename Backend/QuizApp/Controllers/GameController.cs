using Microsoft.AspNetCore.Mvc;
using QuizApp.Data;
using QuizApp.Models;
using QuizApp.DTOs;
using QuizApp.Mapper;
namespace QuizApp.Controllers
{
    [ApiController]
    [Route("play")]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext _context;
        public GameController(AppDbContext context)
        {
            _context = context;
        }

        public class PlayRequest
        {
            public int PlayerId { get; set; }
            public int QuestionId { get; set; }
            public string Answer { get; set; }
            public int TimeTakenSeconds { get; set; }
        }


        [HttpPost]
        public IActionResult Play([FromBody] PlayRequest request)
        {
            var player = _context.Players.Find(request.PlayerId);
            var question = _context.Questions.Find(request.QuestionId);
            if(player == null || question == null)
            {
                return BadRequest("Player or Question not found.");
            }
            bool isCorrect = question.CorrectAnswer.Equals(request.Answer, StringComparison.OrdinalIgnoreCase);

            int points = isCorrect ? 10 + Math.Max(0, 5 - request.TimeTakenSeconds)  : 0;

            var score = new Score
            {
                PlayerId = request.PlayerId,
                QuestionId = request.QuestionId,
                Points = points,
                TimeTakenSeconds = request.TimeTakenSeconds,
                IsCorrect = isCorrect
            };

            _context.Scores.Add(score);
            player.TotalScore += points;
            _context.SaveChanges();

            return Ok(new PlayResponse
            {
                Correct = isCorrect,
                PointsEarned = points,
                TotalScore = player.TotalScore,
                CorrectAnswer = question.CorrectAnswer
            });
        }
        [HttpGet("questions")]
        public IActionResult GetQuestionsForPlay(int count = 10)
        {
            var questions = _context.Questions.ToList()
                .OrderBy(q => Guid.NewGuid())
                .Take(count)
                .Select(q => QuestionMapper.ToPlayDTO(q));
                
                
            return Ok(questions);
        }

        
    }
}
