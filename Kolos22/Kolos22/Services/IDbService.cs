using Kolos22.DTOs;

namespace Kolos22.Services;

public interface IDbService
{
    Task<OrderDTO> GetOrderById(int orderId);
    Task<OrderDTO> FulfillOrder(int orderId, FulfillOrderDTO fulfillOrderDTO);
}