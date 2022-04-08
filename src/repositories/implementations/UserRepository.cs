using Profile.Context;
using Profile.Models;

namespace Profile.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        AppDbContext context = new AppDbContext();


        public List<User> ListUsers()
        {
            return context.users.ToList();
        }

        public User? findById(int id)
        {
            return context.users.Find(id);
        }

        public User register(User user)
        {
            context.users.Add(user);
            context.SaveChanges();

            return user;
        }

        public void update()
        {
            context.SaveChanges();
        }

        public void delete(User user)
        {
            context.users.Remove(user);
            context.SaveChanges();
        }
    }
}