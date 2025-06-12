namespace Kolos22.DTOs;

public class OrderDTO
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FulfilledAt { get; set; }
    public string Status { get; set; } = null!;
    public ClientInfoDTO Client { get; set; }
    public List<OrderLineItemDTO> Products { get; set; } = null!;
}

public class ClientInfoDTO
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}

public class OrderLineItemDTO
{
    public string Name { get; set; } = null!;
    public double Price { get; set; }
    public int Amount { get; set; }
}