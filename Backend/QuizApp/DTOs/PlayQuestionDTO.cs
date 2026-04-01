namespace QuizApp.DTOs
{
    public class PlayQuestionDTO
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public List<string> Answers { get; set; }
    }
}
