namespace QuizApp.DTOs
{
    public class QuestionApiResponse
    {
        public int Response_Code { get; set; }
        public List<QuestionDTO> Results { get; set; }
    }
}
