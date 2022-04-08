using Profile.Repository;
using Profile.Models;

public class FindUsersById
{
    private IUserRepository userRepository;

    public FindUsersById(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public User? execute(int id)
    {
        return this.userRepository.findById(id);
    }
}