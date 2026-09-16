using SneakerStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace SneakerStore.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Sneaker> Sneakers { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<FavoriteItem> FavoriteItems { get; set; }
}
