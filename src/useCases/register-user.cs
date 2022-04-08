using Profile.Repository;
using Profile.Services;
using Profile.ViewModels;
using Profile.Models;

public class RegisterUser
{
    private IUserRepository userRepository;

    public RegisterUser(IUserRepository userRepository) {
        this.userRepository = userRepository;
    }
    
    public object execute(UserViewModel model)
    {
        User user = model.MapTo();
        userRepository.register(user);

        var token = TokenService.GenerateToken(user);

        return new { user, token };
    }
}