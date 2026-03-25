namespace QuizApp.Models
{
    public class Question
    {
        public int Id { get; set; }

        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }

        public string Text { get; set; }
        public string CorrectAnswer { get; set; }
        public string IncorrectAnswersJson { get; set; }
    }
}
