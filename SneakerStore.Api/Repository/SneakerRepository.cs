using SneakerStore.Api.Data;
using SneakerStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace SneakerStore.Api.Repository;

public class SneakerRepository : ISneakerRepository
{
    private readonly ApplicationDbContext _context;

    public SneakerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sneaker>> GetAll()
    {
        return await _context.Sneakers.ToListAsync();
    }

    public async Task<Sneaker?> GetById(int id)
    {
        return await _context.Sneakers.FindAsync(id);
    }

    public async Task<Sneaker> Add(Sneaker sneaker)
    {
        _context.Sneakers.Add(sneaker);
        await _context.SaveChangesAsync();
        return sneaker;
    }

    public async Task Update(Sneaker sneaker)
    {
        _context.Sneakers.Update(sneaker);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var sneaker = await _context.Sneakers.FindAsync(id);
        if (sneaker == null) return;
        _context.Sneakers.Remove(sneaker);
        await _context.SaveChangesAsync();
    }
}
