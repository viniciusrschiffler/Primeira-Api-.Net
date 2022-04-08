using System.Security.Claims;
using Profile.Repository;

public class DeleteUserImage
{

    private IUserRepository userRepository;
    private IDeleteFile deleteFile;
    
    public DeleteUserImage(
        IUserRepository userRepository,
        IDeleteFile deleteFile){

        this.userRepository = userRepository;
        this.deleteFile = deleteFile;
    }

    public bool execute(ClaimsPrincipal claims) 
    {
        if (claims.Identity is null || claims.Identity.Name is null) return false;

        var user = this.userRepository.findById(Int32.Parse(claims.Identity.Name));
        if (user is null) return false;


        if (user.image_path != null)
        {
            var deleted = this.deleteFile.execute(user.image_path);
            if (!deleted) return false;

            user.image_path = null;
            this.userRepository.update();
        }

        return true;
    }
}