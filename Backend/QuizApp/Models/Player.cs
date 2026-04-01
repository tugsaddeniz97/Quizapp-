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


        public override bool Equals(object? obj)
        {
            if (obj is Player other)
            {
                return this.Username == other.Username &&
                       this.TotalScore == other.TotalScore;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username, TotalScore);
        }
    }
}