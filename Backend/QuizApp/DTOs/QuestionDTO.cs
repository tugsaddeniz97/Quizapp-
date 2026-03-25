namespace QuizApp.DTOs
{
    public class QuestionDTO
    {
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
        public string Question { get; set; }
        public string Correct_Answer { get; set; }
        public List<string> Incorrect_Answers { get; set; }
    }
}
