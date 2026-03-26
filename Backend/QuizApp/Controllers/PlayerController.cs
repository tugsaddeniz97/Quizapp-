using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Data;

namespace QuizApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlayerController(AppDbContext context)
        {
            _context = context;
        }

        public class CreatePlayerRequest
        {
            public string UserName { get; set; }
        }

        [HttpPost]
        public IActionResult CreatePlayer([FromBody] CreatePlayerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                return BadRequest("Player name cannot be empty.");
            }
            var player = new Models.Player
            {
                Username = request.UserName,
                
            };
            _context.Players.Add(player);
            _context.SaveChanges();
            return Ok(player);
        }
    }

}