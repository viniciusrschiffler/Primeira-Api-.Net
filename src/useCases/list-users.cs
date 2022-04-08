using Profile.Repository;
using Profile.Models;

public class ListUsers
{
    private IUserRepository userRepository;

    public ListUsers(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public List<User> execute()
    {
        return this.userRepository.ListUsers();
    }
}