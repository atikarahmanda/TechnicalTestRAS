namespace FinalProjectRAS.ViewModels
{
    public class QuestionsVM
    {
        public string Question_id { get; set; }
        public string Question_text { get; set; }
        public string ImagePath { get; set; }
        public List<OptionVM> Options { get; set; }
    }
    public class OptionVM
    {
        public string Option_Id { get; set; }
        public string Option_Text { get; set; }
        public string Option_Image { get; set; }
    }
    public class AnswerVM
    {
        public Guid User_id { get; set; }
        public string Question_id { get; set; }
        public string Option_id { get; set; }
    }
    public class UserScoreVM
    {
        public Guid UserId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public int TotalScore { get; set; }
    }

}

