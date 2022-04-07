using Profile.Context;
using Profile.ViewModels;
using System.Security.Claims;

using Microsoft.Extensions.FileProviders;

namespace Profile.Services
{
    public static class UserService
    {
        public static string login()
        {
            AppDbContext context = new AppDbContext();
            // var user = context.users.FirstOrDefault(x => x.email == "
            var user = context.users.Find(5);
            if (user is null) return null;

            return TokenService.GenerateToken(user);
        }

        public static object register(UserViewModel model)
        {
            AppDbContext context = new AppDbContext();

            User user = model.MapTo();
            context.users.Add(user);
            context.SaveChanges();

            var token = TokenService.GenerateToken(user);

            return new { user, token };
        }

        public static User? Update(ClaimsPrincipal claims, UserViewModel model)
        {
            AppDbContext context = new AppDbContext();

            var user = context.users.Find(Int32.Parse(claims.Identity.Name));
            if (user is null) return null;

            user.name = model.name;
            context.SaveChanges();

            return user;
        }

        public static bool Delete(ClaimsPrincipal claims, string contentRootPath)
        {
            AppDbContext context = new AppDbContext();

            var user = context.users.Find(Int32.Parse(claims.Identity.Name));
            if (user is null) return false;

            if (user.image_path != null)
            {
                File.Delete(Path.Combine(contentRootPath, "Uploads", user.image_path));
            }

            context.users.Remove(user);
            context.SaveChanges();

            return true;
        }

        public static List<User> getAll()
        {
            AppDbContext context = new AppDbContext();

            return context.users.ToList();
        }

        public static User? getById(int id)
        {
            AppDbContext context = new AppDbContext();

            return context.users.Find(id);
        }

        public static async Task<string?> UploadImage(HttpRequest request, ClaimsPrincipal claims, string contentRootPath)
        {
            AppDbContext context = new AppDbContext();
            Random randNum = new Random();

            var form = await request.ReadFormAsync();
            var file = form.Files["file"];

            if (file is null || file.Length == 0) return null;

            var user = context.users.Find(Int32.Parse(claims.Identity.Name));
            if (user is null) return null;

            var originalFileName = file.FileName;

            var uniqueFileName = $"{randNum.Next()}-{originalFileName}";
            var uniqueFilePath = Path.Combine("./", "Uploads", uniqueFileName);

            if (user.image_path != null)
            {
                File.Delete(Path.Combine(contentRootPath, "Uploads", user.image_path));
            }

            using (var stream = File.Create(uniqueFilePath))
            {
                await file.CopyToAsync(stream);
            }

            user.image_path = uniqueFileName;
            context.SaveChanges();

            return $"http://localhost:5036/images/{uniqueFileName}";
        }

        public static bool DeleteImage(ClaimsPrincipal claims, string contentRootPath)
        {
            AppDbContext context = new AppDbContext();

            var user = context.users.Find(Int32.Parse(claims.Identity.Name));
            if (user is null) return false;

            if (user.image_path != null)
            {
                File.Delete(Path.Combine(contentRootPath, "Uploads", user.image_path));
                user.image_path = null;
                context.SaveChanges();
            }

            return true;
        }
    }
}