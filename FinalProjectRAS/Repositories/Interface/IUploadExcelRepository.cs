namespace FinalProjectRAS.Repositories.Interface
{
    public interface IUploadExcelRepository
    {
        (string Message, int SavedQuestionsCount) SaveQuestionsFromExcel(Stream fileStream);
    }
}
