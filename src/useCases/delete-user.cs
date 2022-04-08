using System.Security.Claims;
using Profile.Repository;

public class DeleteUser
{
    private IUserRepository userRepository;

    public DeleteUser(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public bool execute(ClaimsPrincipal claims)
    {
        if (claims.Identity is null || claims.Identity.Name is null) return false;

        var user = this.userRepository.findById(Int32.Parse(claims.Identity.Name));
        if (user is null) return false;

        if (user.image_path != null)
        {
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Uploads", user.image_path));
        }

        this.userRepository.delete(user);

        return true;
    }
}