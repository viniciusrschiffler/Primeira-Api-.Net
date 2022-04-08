using Profile.Models;

namespace Profile.Repository
{
    public interface IUserRepository
    {
        List<User> ListUsers();
        User? findById(int id);
        User register(User user);
        void update();
        void delete(User user);
    }
}