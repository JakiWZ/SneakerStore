using Microsoft.EntityFrameworkCore;
using SneakerStore.Api.Data;
using SneakerStore.Api.Models;

namespace SneakerStore.Api.Services;

public class FavoriteService : IFavoriteService
{
    private readonly ApplicationDbContext _context;

    public FavoriteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sneaker>> GetFavorites(int userId)
    {
        return await (
            from favorite in _context.FavoriteItems
            join sneaker in _context.Sneakers on favorite.SneakerId equals sneaker.Id
            where favorite.UserId == userId
            select sneaker
        ).ToListAsync();
    }

    public async Task<(bool Success, string Error)> AddFavorite(int userId, int sneakerId)
    {
        var sneakerExists = await _context.Sneakers.AnyAsync(s => s.Id == sneakerId);
        if (!sneakerExists)
            return (false, "Sneaker not found.");

        var exists = await _context.FavoriteItems.AnyAsync(f => f.UserId == userId && f.SneakerId == sneakerId);
        if (exists)
            return (true, string.Empty);

        _context.FavoriteItems.Add(new FavoriteItem { UserId = userId, SneakerId = sneakerId });
        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task RemoveFavorite(int userId, int sneakerId)
    {
        var existing = await _context.FavoriteItems.SingleOrDefaultAsync(f => f.UserId == userId && f.SneakerId == sneakerId);
        if (existing == null) return;
        _context.FavoriteItems.Remove(existing);
        await _context.SaveChangesAsync();
    }
}
