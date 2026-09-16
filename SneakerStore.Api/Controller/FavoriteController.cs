using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerStore.Api.Models;
using SneakerStore.Api.Services;

namespace SneakerStore.Api.Controller;

// api/Favorite
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoriteController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    private int UserId => int.Parse(User.FindFirstValue("uid")!);

    // GET api/Favorite
    [HttpGet]
    public async Task<List<Sneaker>> Get()
    {
        return await _favoriteService.GetFavorites(UserId);
    }

    // POST api/Favorite/1
    [HttpPost("{sneakerId}")]
    public async Task<IActionResult> Add(int sneakerId)
    {
        var (success, error) = await _favoriteService.AddFavorite(UserId, sneakerId);
        if (!success)
            return BadRequest(new { error });
        return Ok();
    }

    // DELETE api/Favorite/1
    [HttpDelete("{sneakerId}")]
    public async Task<IActionResult> Remove(int sneakerId)
    {
        await _favoriteService.RemoveFavorite(UserId, sneakerId);
        return Ok();
    }
}
