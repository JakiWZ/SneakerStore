using SneakerStore.Api.Models;

namespace SneakerStore.Api.Repository;

public interface ISneakerRepository
{
    Task<List<Sneaker>> GetAll();

    Task<Sneaker?> GetById(int id);

    Task<Sneaker> Add(Sneaker sneaker);

    Task Update(Sneaker sneaker);

    Task Delete(int id);
}
