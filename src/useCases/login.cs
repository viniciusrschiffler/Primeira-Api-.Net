using Profile.Repository;
using Profile.Services;

public class Login
{
    private IUserRepository userRepository;

    public Login(IUserRepository userRepository) {
        this.userRepository = userRepository;
    }
    
    public string execute(int id)
    {
        var user = this.userRepository.findById(id);

        if (user is null) throw new Exception();

        return TokenService.GenerateToken(user);
    }
}