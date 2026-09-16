using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SneakerStore.Api.Data;
using SneakerStore.Api.Services;

namespace SneakerStore.Tests;

public class AuthServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static IConfiguration CreateConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "SneakerStore.Tests",
            ["Jwt:Audience"] = "SneakerStore.Tests",
            ["Jwt:Key"] = "unit-test-signing-key-with-enough-length-123!"
        };
        return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
    }

    [Fact]
    public async Task Register_WithValidData_CreatesUser()
    {
        var context = CreateContext();
        var service = new AuthService(context, CreateConfiguration());

        var (success, error) = await service.Register("newuser", "Password1", "User");

        Assert.True(success);
        Assert.Empty(error);
        Assert.True(await context.Users.AnyAsync(u => u.Username == "newuser"));
    }

    [Fact]
    public async Task Register_WithDuplicateUsername_Fails()
    {
        var context = CreateContext();
        var service = new AuthService(context, CreateConfiguration());
        await service.Register("duplicate", "Password1", "User");

        var (success, error) = await service.Register("duplicate", "Password2", "User");

        Assert.False(success);
        Assert.Equal("Username already taken.", error);
    }

    [Theory]
    [InlineData("short1")]
    [InlineData("nouppercase1")]
    [InlineData("NoDigitsHere")]
    public async Task Register_WithWeakPassword_Fails(string weakPassword)
    {
        var context = CreateContext();
        var service = new AuthService(context, CreateConfiguration());

        var (success, error) = await service.Register("someuser", weakPassword, "User");

        Assert.False(success);
        Assert.Contains("Password", error);
    }

    [Fact]
    public async Task Register_WithInvalidRole_Fails()
    {
        var context = CreateContext();
        var service = new AuthService(context, CreateConfiguration());

        var (success, error) = await service.Register("someuser", "Password1", "SuperAdmin");

        Assert.False(success);
        Assert.Equal("Invalid role.", error);
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsToken()
    {
        var context = CreateContext();
        var service = new AuthService(context, CreateConfiguration());
        await service.Register("loginuser", "Password1", "User");

        var (success, token, role, error) = await service.Login("loginuser", "Password1");

        Assert.True(success);
        Assert.NotEmpty(token);
        Assert.Equal("User", role);
        Assert.Empty(error);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Fails()
    {
        var context = CreateContext();
        var service = new AuthService(context, CreateConfiguration());
        await service.Register("loginuser2", "Password1", "User");

        var (success, token, _, error) = await service.Login("loginuser2", "WrongPassword1");

        Assert.False(success);
        Assert.Empty(token);
        Assert.Equal("Invalid credentials.", error);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_Fails()
    {
        var context = CreateContext();
        var service = new AuthService(context, CreateConfiguration());

        var (success, _, _, error) = await service.Login("ghost", "Password1");

        Assert.False(success);
        Assert.Equal("Invalid credentials.", error);
    }
}
