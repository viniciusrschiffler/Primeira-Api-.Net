using Profile.ViewModels;

namespace Profile.Repository
{
    public interface IDeleteFile
    {
        bool execute(string fileName);
    }
}