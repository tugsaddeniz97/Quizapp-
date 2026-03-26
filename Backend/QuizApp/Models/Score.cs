using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Score
    {
        [Key]
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public bool IsCorrect { get; set; }
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TimeTakenSeconds { get; set; }
    }
}
