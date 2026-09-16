using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerStore.Api.DTO;
using SneakerStore.Api.Services;

namespace SneakerStore.Api.Controller;

// api/Cart
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private int UserId => int.Parse(User.FindFirstValue("uid")!);

    // GET api/Cart
    [HttpGet]
    public async Task<CartView> Get()
    {
        return await _cartService.GetCart(UserId);
    }

    // POST api/Cart
    [HttpPost]
    public async Task<IActionResult> Add(AddCartItemRequest request)
    {
        var (success, error) = await _cartService.AddItem(UserId, request.SneakerId, request.Quantity);
        if (!success)
            return BadRequest(new { error });
        return Ok();
    }

    // PUT api/Cart/1
    [HttpPut("{sneakerId}")]
    public async Task<IActionResult> SetQuantity(int sneakerId, UpdateCartItemRequest request)
    {
        var (success, error) = await _cartService.SetQuantity(UserId, sneakerId, request.Quantity);
        if (!success)
            return BadRequest(new { error });
        return Ok();
    }

    // DELETE api/Cart/1
    [HttpDelete("{sneakerId}")]
    public async Task<IActionResult> Remove(int sneakerId)
    {
        await _cartService.RemoveItem(UserId, sneakerId);
        return Ok();
    }

    // DELETE api/Cart
    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        await _cartService.Clear(UserId);
        return Ok();
    }
}
