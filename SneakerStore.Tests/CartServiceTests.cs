using Microsoft.EntityFrameworkCore;
using SneakerStore.Api.Data;
using SneakerStore.Api.Models;
using SneakerStore.Api.Services;

namespace SneakerStore.Tests;

public class CartServiceTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static async Task<ApplicationDbContext> SeedSneaker(ApplicationDbContext context, int id, decimal price)
    {
        context.Sneakers.Add(new Sneaker
        {
            Id = id,
            Name = "Test Sneaker",
            Brand = "TestBrand",
            Model = "T1",
            Size = "42",
            QuantityAvailable = 10,
            Price = price
        });
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task AddItem_WithZeroQuantity_Fails()
    {
        var context = await SeedSneaker(CreateContext(), 1, 100m);
        var service = new CartService(context);

        var (success, error) = await service.AddItem(userId: 1, sneakerId: 1, quantity: 0);

        Assert.False(success);
        Assert.Equal("Quantity must be greater than zero.", error);
    }

    [Fact]
    public async Task AddItem_WithNonExistentSneaker_Fails()
    {
        var context = CreateContext();
        var service = new CartService(context);

        var (success, error) = await service.AddItem(userId: 1, sneakerId: 999, quantity: 1);

        Assert.False(success);
        Assert.Equal("Sneaker not found.", error);
    }

    [Fact]
    public async Task AddItem_Twice_AccumulatesQuantity()
    {
        var context = await SeedSneaker(CreateContext(), 1, 100m);
        var service = new CartService(context);

        await service.AddItem(userId: 1, sneakerId: 1, quantity: 2);
        await service.AddItem(userId: 1, sneakerId: 1, quantity: 3);

        var cart = await service.GetCart(1);
        Assert.Single(cart.Items);
        Assert.Equal(5, cart.Items[0].Quantity);
    }

    [Fact]
    public async Task GetCart_ComputesLineTotalsAndTotal()
    {
        var context = await SeedSneaker(CreateContext(), 1, 50m);
        var service = new CartService(context);
        await service.AddItem(userId: 1, sneakerId: 1, quantity: 3);

        var cart = await service.GetCart(1);

        Assert.Equal(150m, cart.Items[0].LineTotal);
        Assert.Equal(150m, cart.Total);
    }

    [Fact]
    public async Task SetQuantity_ToZero_RemovesItem()
    {
        var context = await SeedSneaker(CreateContext(), 1, 50m);
        var service = new CartService(context);
        await service.AddItem(userId: 1, sneakerId: 1, quantity: 2);

        var (success, _) = await service.SetQuantity(userId: 1, sneakerId: 1, quantity: 0);
        var cart = await service.GetCart(1);

        Assert.True(success);
        Assert.Empty(cart.Items);
    }

    [Fact]
    public async Task SetQuantity_WithNegativeValue_Fails()
    {
        var context = await SeedSneaker(CreateContext(), 1, 50m);
        var service = new CartService(context);
        await service.AddItem(userId: 1, sneakerId: 1, quantity: 2);

        var (success, error) = await service.SetQuantity(userId: 1, sneakerId: 1, quantity: -1);

        Assert.False(success);
        Assert.Equal("Quantity cannot be negative.", error);
    }

    [Fact]
    public async Task Clear_RemovesAllItemsForUser()
    {
        var context = await SeedSneaker(CreateContext(), 1, 50m);
        await SeedSneaker(context, 2, 75m);
        var service = new CartService(context);
        await service.AddItem(userId: 1, sneakerId: 1, quantity: 1);
        await service.AddItem(userId: 1, sneakerId: 2, quantity: 1);

        await service.Clear(1);
        var cart = await service.GetCart(1);

        Assert.Empty(cart.Items);
    }
}
