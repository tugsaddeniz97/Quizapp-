using Microsoft.AspNetCore.Mvc;
using QuizApp.Data;
using QuizApp.DTOs;
using QuizApp.Mapper;
using System.Text.Json;

namespace QuizApp.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetQuestions()
        {
            var questions = _context.Questions.ToList().Select(q => QuestionMapper.ToDTO(q));
            return Ok(questions);
        }
        [HttpPost]
        public IActionResult AddQuestion(QuestionDTO dto)
        {
            var question = QuestionMapper.ToEntity(dto);
            _context.Questions.Add(question);
            _context.SaveChanges();
            return Ok(question);
        }
        [HttpPost("import")]
        public IActionResult ImportQuestions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var json = System.IO.File.ReadAllText("data/questionsData.json");
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<QuestionApiResponse>(json, options);

            if (apiResponse?.Results == null)
                return BadRequest("Invalid JSON format.");

            var questionDTO = apiResponse.Results;
            if (questionDTO == null) return BadRequest("Invalid JSON format.");
            
            var questions = questionDTO.Select(q => QuestionMapper.ToEntity(q));
            _context.Questions.AddRange(questions);
            _context.SaveChanges();
            
           
            return Ok(questions);

        }
    }
}
