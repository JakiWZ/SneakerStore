using Microsoft.EntityFrameworkCore;
using SneakerStore.Api.Data;
using SneakerStore.Api.DTO;
using SneakerStore.Api.Models;

namespace SneakerStore.Api.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartView> GetCart(int userId)
    {
        var items = await (
            from cartItem in _context.CartItems
            join sneaker in _context.Sneakers on cartItem.SneakerId equals sneaker.Id
            where cartItem.UserId == userId
            select new CartItemView
            {
                SneakerId = sneaker.Id,
                Name = sneaker.Name,
                Brand = sneaker.Brand,
                Price = sneaker.Price,
                Quantity = cartItem.Quantity,
                LineTotal = sneaker.Price * cartItem.Quantity
            }
        ).ToListAsync();

        return new CartView { Items = items, Total = items.Sum(i => i.LineTotal) };
    }

    public async Task<(bool Success, string Error)> AddItem(int userId, int sneakerId, int quantity)
    {
        if (quantity <= 0)
            return (false, "Quantity must be greater than zero.");

        var sneakerExists = await _context.Sneakers.AnyAsync(s => s.Id == sneakerId);
        if (!sneakerExists)
            return (false, "Sneaker not found.");

        var existing = await _context.CartItems.SingleOrDefaultAsync(c => c.UserId == userId && c.SneakerId == sneakerId);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _context.CartItems.Add(new CartItem { UserId = userId, SneakerId = sneakerId, Quantity = quantity });
        }

        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task<(bool Success, string Error)> SetQuantity(int userId, int sneakerId, int quantity)
    {
        if (quantity < 0)
            return (false, "Quantity cannot be negative.");

        var existing = await _context.CartItems.SingleOrDefaultAsync(c => c.UserId == userId && c.SneakerId == sneakerId);
        if (existing == null)
            return (false, "Item not in cart.");

        if (quantity == 0)
        {
            _context.CartItems.Remove(existing);
        }
        else
        {
            existing.Quantity = quantity;
        }

        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    public async Task RemoveItem(int userId, int sneakerId)
    {
        var existing = await _context.CartItems.SingleOrDefaultAsync(c => c.UserId == userId && c.SneakerId == sneakerId);
        if (existing == null) return;
        _context.CartItems.Remove(existing);
        await _context.SaveChangesAsync();
    }

    public async Task Clear(int userId)
    {
        var items = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}
