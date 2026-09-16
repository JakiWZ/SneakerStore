using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SneakerStore.Api.Data;
using SneakerStore.Api.Models;
using SneakerStore.Api.Repository;
using SneakerStore.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddScoped<ISneakerRepository, SneakerRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

var jwtSection = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// create the database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    var adminSection = app.Configuration.GetSection("AdminSeed");
    var adminUsername = adminSection["Username"];
    var adminPassword = adminSection["Password"];

    if (!string.IsNullOrEmpty(adminUsername) && !context.Users.Any(u => u.Username == adminUsername))
    {
        context.Users.Add(new User
        {
            Username = adminUsername,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Role = "Admin"
        });
        context.SaveChanges();
    }

    if (!context.Sneakers.Any())
    {
        context.Sneakers.AddRange(
            new Sneaker { Name = "Air Max 90", Brand = "Nike", Model = "AM90", Size = "42", QuantityAvailable = 10, Price = 129.99m },
            new Sneaker { Name = "Superstar", Brand = "Adidas", Model = "SS", Size = "41", QuantityAvailable = 8, Price = 89.99m },
            new Sneaker { Name = "Old Skool", Brand = "Vans", Model = "OS", Size = "43", QuantityAvailable = 6, Price = 74.50m },
            new Sneaker { Name = "574 Core", Brand = "New Balance", Model = "574", Size = "44", QuantityAvailable = 5, Price = 99.00m },
            new Sneaker { Name = "Chuck Taylor All Star", Brand = "Converse", Model = "CT70", Size = "40", QuantityAvailable = 12, Price = 69.99m }
        );
        context.SaveChanges();
    }
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
    });
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
