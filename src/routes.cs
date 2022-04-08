
using System.Security.Claims;
using Profile.Repository.Implementations;
using Profile.ViewModels;

namespace Profile.Controller
{
    public static class Routes
    {
        public static void MapRoutes(this WebApplication app)
        {
            UserRepository userRepository = new UserRepository();

            app.MapGet("/", () =>
            {
                ListUsers listUsers = new ListUsers(userRepository);
                return Results.Ok(listUsers.execute());
            });
            // Metodo anônimo. não precisa definir como anônimo
            // Caso tenha usado uma politica global de autorização, pode se usar .AllowAnonymous()
            

            app.MapGet("/{id:int}", (int id) =>
            {
                FindUsersById findUsersById = new FindUsersById(userRepository);
                var user = findUsersById.execute(id);

                return user is not null ? Results.Ok(user) : Results.NotFound();
            });


            app.MapPost("/login/{id:int}", (int id) =>
            {
                Login login = new Login(userRepository);
                var token = login.execute(id);

                return Results.Ok(new { token });
            });
            

            app.MapPost("/", (UserViewModel model) =>
            {
                RegisterUser registerUser = new RegisterUser(userRepository);
                var response = registerUser.execute(model);
                
                return Results.Created("", response);
            });


            app.MapPut("/", (ClaimsPrincipal claims, UserViewModel model) =>
            {
                UpdateUser updateUser = new UpdateUser(userRepository);
                var updatedUser = updateUser.execute(claims, model);

                return updatedUser is not null ? Results.Ok(updatedUser) : Results.BadRequest();
            }).RequireAuthorization();


            app.MapDelete("/", (ClaimsPrincipal claims) =>
            {
                DeleteUser deleteUser = new DeleteUser(userRepository);
                var deleted = deleteUser.execute(claims);

                return deleted ? Results.NoContent() : Results.NotFound();
            }).RequireAuthorization();


            app.MapPost("/image", async (HttpRequest request, ClaimsPrincipal claims) =>
            {
                SaveUserImage saveUserImage = new SaveUserImage(userRepository, new UploadFile(), new DeleteFile());
                var image_url = await saveUserImage.execute(request, claims);

                return image_url is null ? Results.BadRequest() : Results.Ok(new { image_url });

            }).RequireAuthorization();

            app.MapDelete("/image", (ClaimsPrincipal claims) =>
            {
                DeleteUserImage deleteUserImage = new DeleteUserImage(userRepository, new DeleteFile());
                var deleted = deleteUserImage.execute(claims);

                return deleted ? Results.NoContent() : Results.BadRequest();
            }).RequireAuthorization();
        }
    }
}