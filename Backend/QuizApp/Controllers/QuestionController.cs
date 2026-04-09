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
        private readonly IHttpClientFactory _httpClientFactory;

        public QuestionsController(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
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

        [HttpPost("fetch")]
        public async Task<IActionResult> FetchQuestionsFromApi([FromQuery] int amount = 10)
        {
            if (amount < 1 || amount > 50)
            {
                return BadRequest("Amount must be between 1 and 50.");
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"https://opentdb.com/api.php?amount={amount}&type=multiple");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Failed to fetch questions from external API.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = JsonSerializer.Deserialize<QuestionApiResponse>(content, options);

                if (apiResponse?.Results == null || apiResponse.Response_Code != 0)
                {
                    return BadRequest("Invalid response from external API.");
                }

                var questions = apiResponse.Results.Select(q => QuestionMapper.ToEntity(q));
                _context.Questions.AddRange(questions);
                _context.SaveChanges();

                return Ok(new
                {
                    Message = $"Successfully fetched and saved {apiResponse.Results.Count} questions.",
                    Questions = apiResponse.Results
                });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching questions: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
