using System.Security.Claims;
using Profile.Repository;

public class SaveUserImage
{

    private IUserRepository userRepository;
    private IUploadFile uploadFile;
    private IDeleteFile deleteFile;
    public SaveUserImage(
        IUserRepository userRepository, 
        IUploadFile uploadFile, 
        IDeleteFile deleteFile){

        this.userRepository = userRepository;
        this.uploadFile = uploadFile;
        this.deleteFile = deleteFile;
    }

    public async Task<string?> execute(HttpRequest request, ClaimsPrincipal claims) 
    {
        if (claims.Identity is null || claims.Identity.Name is null) return null;

        var user = this.userRepository.findById(Int32.Parse(claims.Identity.Name));
        if (user is null) return null;

        if (user.image_path != null)
        {
            var deleted = this.deleteFile.execute(user.image_path);
            if (!deleted) return null;
        }

        var imageName = await this.uploadFile.execute(request);

        user.image_path = imageName;
        this.userRepository.update();

        return $"http://localhost:5036/images/{imageName}";
    }
}