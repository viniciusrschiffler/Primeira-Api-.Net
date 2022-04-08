using Profile.Repository;

public class UploadFile : IUploadFile
{
    public UploadFile() { }

    public async Task<string?> execute(HttpRequest request) 
    {
        if(!request.HasFormContentType) return null;
        
        Random randNum = new Random();

        var form = await request.ReadFormAsync();
        var file = form.Files["file"];

        if (file is null || file.Length == 0) return null;


        var originalFileName = file.FileName;

        var uniqueFileName = $"{randNum.Next()}-{originalFileName}";
        var uniqueFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", uniqueFileName);


        using (var stream = File.Create(uniqueFilePath))
        {
            await file.CopyToAsync(stream);
        }


        return uniqueFileName;
    }
}