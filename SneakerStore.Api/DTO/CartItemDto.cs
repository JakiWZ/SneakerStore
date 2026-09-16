namespace SneakerStore.Api.DTO;

public class CartItemView
{
    public int SneakerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public class CartView
{
    public List<CartItemView> Items { get; set; } = new();
    public decimal Total { get; set; }
}

public class AddCartItemRequest
{
    public int SneakerId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    public int Quantity { get; set; }
}
