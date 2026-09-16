namespace SneakerStore.Api.DTO;

public class SneakerDto
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public int QuantityAvailable { get; set; }
    public decimal Price { get; set; }
}
