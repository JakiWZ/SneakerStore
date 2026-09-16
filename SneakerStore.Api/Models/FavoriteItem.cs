using System.ComponentModel.DataAnnotations;

namespace SneakerStore.Api.Models;

public class FavoriteItem
{
    [Key] public int Id { get; set; }

    public int UserId { get; set; }

    public int SneakerId { get; set; }
}
