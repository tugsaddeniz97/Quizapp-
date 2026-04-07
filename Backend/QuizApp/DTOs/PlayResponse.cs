namespace QuizApp.DTOs
{
    public class PlayResponse
    {
        public bool Correct { get; set; }
        public int PointsEarned { get; set; }
        public string CorrectAnswer { get; set; }
        public int TotalScore { get; set; }
    }
}
