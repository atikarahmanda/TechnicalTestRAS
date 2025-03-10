using FinalProjectRAS.ViewModels;

namespace FinalProjectRAS.Repositories.Interface
{
    public interface IQuestionsRepository
    {
        Task<List<QuestionsVM>> GetRandomQuestionsByLevel();
        Task<List<QuestionsVM>> GetAllQuestion();
        Task<bool> SaveAnswer(AnswerVM answerVM);
        Task<int> GetTotalScoreByUserId(Guid userId);
        Task<List<UserScoreVM>> GetAllUserScores();
        StatisticQuestionVM GetStatisticQuestion();
        int DeleteQuestion(string Question_id);
    }
}