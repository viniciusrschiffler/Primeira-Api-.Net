using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

using Profile.ViewModels;
using Profile.Context;
using Profile.Services;
using Profile;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();

// configurando cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("*");
                      });
});



var key = Encoding.ASCII.GetBytes(Settings.secret); // Vai transformar a chave em um array de bytes
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddAuthorization(
// options =>
// {
//     options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
//     options.AddPolicy("User", policy => policy.RequireRole("User"));
// }
);

var app = builder.Build();

// usando a configuração do cors
app.UseCors(MyAllowSpecificOrigins);

// Define o rota para as imagens
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/images"
}
);

// Configurando para user autorização e autenticação
// Deve ser nessa ordem, Authentication depois Authorization
app.UseAuthentication();
app.UseAuthorization();


app.MapPost("/login", () =>
{
    var token = UserService.login();
    if (token is null) return Results.NotFound();

    return Results.Ok(new { token });
}).AllowAnonymous();

app.MapGet("/", () =>
{
    Console.WriteLine(Results.Ok());
    var users = UserService.getAll();
    return Results.Ok(users);
});
// Metodo anônimo. não precisa definir como anônimo
// Caso tenha usado uma politica global de autorização, pode se usar .AllowAnonymous()

app.MapGet("/{id:int}", (int id) =>
{
    var user = UserService.getById(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/", (UserViewModel model) =>
{
    var response = UserService.register(model);
    
    return Results.Created("", response);
});

app.MapPut("/", (ClaimsPrincipal claims, UserViewModel model) =>
{
    if (claims.Identity.Name is null) return Results.Unauthorized();

    var updatedUser = UserService.Update(claims, model);

    return updatedUser is not null ? Results.Ok(updatedUser) : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/", (ClaimsPrincipal claims) =>
{
    if (claims.Identity.Name is null) return Results.Unauthorized();

    var deleted = UserService.Delete(claims, builder.Environment.ContentRootPath);

    return deleted ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.MapPost("/image", async (HttpRequest request, ClaimsPrincipal claims) =>
{
    if (claims.Identity.Name is null) return Results.Unauthorized();
    if (!request.HasFormContentType) return Results.BadRequest();

    var image_url = await UserService.UploadImage(request, claims, builder.Environment.ContentRootPath);

    return image_url is null ? Results.BadRequest() : Results.Ok(new { image_url });

}).RequireAuthorization();

app.MapDelete("/image", (ClaimsPrincipal claims) =>
{
    if (claims.Identity.Name is null) return Results.Unauthorized();

    var deleted = UserService.DeleteImage(claims, builder.Environment.ContentRootPath);

    return deleted ? Results.NoContent() : Results.BadRequest();
}).RequireAuthorization();


app.MapGet("/image", (ClaimsPrincipal claims, AppDbContext context) =>
{
    if (claims.Identity.Name is null) return Results.Unauthorized();

    // var image = UserService.GetImage(Int32.Parse(claims.Identity.Name), builder.Environment.ContentRootPath);
    var user = context.users.Find(Int32.Parse(claims.Identity.Name));
    if (user.image_path is null) return Results.BadRequest();

    var fileBytes = File.ReadAllBytes(Path.Combine(builder.Environment.ContentRootPath, "Uploads", user.image_path));

    return user is null ? Results.NotFound() : Results.File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "assa");
});


app.Run("http://localhost:5036");