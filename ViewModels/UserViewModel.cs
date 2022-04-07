namespace Profile.ViewModels
{
    public class UserViewModel 
    {
        public string name { get; set; }

        public User MapTo() 
        {
            return new User(0, name, null);
        }
    }
}