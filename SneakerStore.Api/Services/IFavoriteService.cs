using SneakerStore.Api.Models;

namespace SneakerStore.Api.Services;

public interface IFavoriteService
{
    Task<List<Sneaker>> GetFavorites(int userId);

    Task<(bool Success, string Error)> AddFavorite(int userId, int sneakerId);

    Task RemoveFavorite(int userId, int sneakerId);
}
