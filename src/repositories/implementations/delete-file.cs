using Profile.Repository;

public class DeleteFile : IDeleteFile
{
    public DeleteFile() { }

    public bool execute(string fileName) 
    {
        try
        {
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName));
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}