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


        public override bool Equals(object? obj)
        {
            if (obj is QuestionDTO other)
            {
                return
                    this.Type == other.Type &&
                    this.Difficulty == other.Difficulty &&
                    this.Category == other.Category &&
                    this.Question == other.Question &&
                    this.Correct_Answer == other.Correct_Answer;

            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Difficulty, Category, Question, Correct_Answer);
        }
    }
}
