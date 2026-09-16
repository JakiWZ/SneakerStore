namespace SneakerStore.Api.Services;

public interface IAuthService
{
    Task<(bool Success, string Error)> Register(string username, string password, string role);

    Task<(bool Success, string Token, string Role, string Error)> Login(string username, string password);
}
