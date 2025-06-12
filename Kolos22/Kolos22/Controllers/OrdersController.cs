using Kolos22.DTOs;
using Kolos22.Exceptions;
using Kolos22.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolos22.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IDbService _dbService;

    public OrdersController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        try
        {
            var order = await _dbService.GetOrderById(id);
            return Ok(order);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPut("{id}/fulfill")]
    public async Task<IActionResult> FulfillOrder(int id, [FromBody] FulfillOrderDTO fulfillOrderDTO)
    {
        try
        {
            var order = await _dbService.FulfillOrder(id, fulfillOrderDTO);
            return Ok(order);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}
