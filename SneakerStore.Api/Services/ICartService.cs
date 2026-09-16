using SneakerStore.Api.DTO;

namespace SneakerStore.Api.Services;

public interface ICartService
{
    Task<CartView> GetCart(int userId);

    Task<(bool Success, string Error)> AddItem(int userId, int sneakerId, int quantity);

    Task<(bool Success, string Error)> SetQuantity(int userId, int sneakerId, int quantity);

    Task RemoveItem(int userId, int sneakerId);

    Task Clear(int userId);
}
