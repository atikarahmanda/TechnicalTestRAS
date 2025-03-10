using FinalProjectRAS.ViewModels;

namespace FinalProjectRAS.Repositories.Interface
{
    public interface IUserRepository
    {
        int Register(RegisterVM register);
        (int, UserVM) Login(LoginVM account);
        IEnumerable<CandidateVM> GetCandidateData();
        IEnumerable<CandidateVM> GetCandidatePastDeadline();
        StatisticVM GetStatisticCandidate();
        int UpdateDeadline(Guid User_id, int newDeadlineDays);
        int DeleteUser(Guid userId);
        
    }
}
