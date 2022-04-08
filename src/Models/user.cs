
namespace Profile.Models 
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string? image_path { get; set; }

        public User(int id, string name, string? image_path) {
            this.id = id;
            this.name = name;
            this.image_path = image_path;
        }
    }
}
