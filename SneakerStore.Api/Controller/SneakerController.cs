using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakerStore.Api.DTO;
using SneakerStore.Api.Models;
using SneakerStore.Api.Repository;

namespace SneakerStore.Api.Controller;

// api/Sneaker
[ApiController]
[Route("api/[controller]")]
public class SneakerController : ControllerBase
{
    private readonly ISneakerRepository _repository;

    public SneakerController(ISneakerRepository repository)
    {
        _repository = repository;
    }

    // GET api/Sneaker
    [HttpGet]
    public async Task<List<Sneaker>> Get()
    {
        return await _repository.GetAll();
    }

    // GET by ID api/Sneaker/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Sneaker>> Get(int id)
    {
        var sneaker = await _repository.GetById(id);
        if (sneaker == null)
            return NotFound();
        return sneaker;
    }

    // POST api/Sneaker
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<Sneaker> Post(SneakerDto dto)
    {
        var sneaker = new Sneaker
        {
            Name = dto.Name,
            Brand = dto.Brand,
            Model = dto.Model,
            Size = dto.Size,
            QuantityAvailable = dto.QuantityAvailable,
            Price = dto.Price
        };
        return await _repository.Add(sneaker);
    }

    // PUT api/Sneaker/1
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(int id, SneakerDto dto)
    {
        var sneaker = new Sneaker
        {
            Id = id,
            Name = dto.Name,
            Brand = dto.Brand,
            Model = dto.Model,
            Size = dto.Size,
            QuantityAvailable = dto.QuantityAvailable,
            Price = dto.Price
        };
        await _repository.Update(sneaker);
        return Ok();
    }

    // DELETE api/Sneaker/1
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.Delete(id);
        return Ok();
    }
}
