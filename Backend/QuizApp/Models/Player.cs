using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TotalScore { get; set; } = 0;
        public List<Score> Scores { get; set; }

    }
}
