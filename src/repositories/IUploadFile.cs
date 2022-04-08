using Profile.ViewModels;

namespace Profile.Repository
{
    public interface IUploadFile
    {
        Task<string?> execute(HttpRequest request);
    }
}