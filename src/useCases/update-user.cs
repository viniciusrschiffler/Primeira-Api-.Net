using System.Security.Claims;
using Profile.Repository;
using Profile.ViewModels;
using Profile.Models;

public class UpdateUser
{
    private IUserRepository userRepository;

    public UpdateUser(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public User? execute(ClaimsPrincipal claims, UserViewModel model)
    {
        if (claims.Identity is null || claims.Identity.Name is null) return null;
        
        var user = this.userRepository.findById(Int32.Parse(claims.Identity.Name));
        if (user is null) return null;

        user.name = model.name;
        this.userRepository.update();

        return user;
    }
}