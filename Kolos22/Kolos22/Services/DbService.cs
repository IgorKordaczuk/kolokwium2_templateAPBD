using System.Data;
using Kolos2.Data;
using Kolos2.Models;
using Kolos22.DTOs;
using Kolos22.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Kolos22.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;

    public DbService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<OrderDTO> GetOrderById(int orderId)
    {
        var order = await _context.Orders
            .Select(e => new OrderDTO
            {
                Id = e.Id,
                CreatedAt = e.CreatedAt,
                FulfilledAt = e.FulfilledAt,
                Status = e.Status.Name,
                Client = new ClientInfoDTO
                {
                    FirstName = e.Client.FirstName,
                    LastName = e.Client.LastName,
                },
                Products = e.ProductOrders.Select(p => new OrderLineItemDTO()
                {
                    Name = p.Product.Name,
                    Price = p.Product.Price,
                    Amount = p.Amount
                }).ToList()
                
            }).FirstOrDefaultAsync(e => e.Id == orderId);

        if (order == null)
            throw new NotFoundException();

        return order;
    }

    public async Task<OrderDTO> FulfillOrder(int orderId, FulfillOrderDTO dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new NotFoundException();
            }

            if (order.FulfilledAt != null)
            {
                throw new ConflictException();
            }
            
            var status = await _context.Statuses.FirstOrDefaultAsync(s => s.Name.Equals(dto.StatusName));
            if (status == null)
            {
                throw new NotFoundException();
            }
            
            order.StatusId = status.Id;
            order.FulfilledAt = DateTime.Now;
            
            var relatedProducts = _context.ProductOrders.Where(o => o.OrderId == orderId);
            _context.ProductOrders.RemoveRange(relatedProducts);
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            var resOrder = await _context.Orders
                .Select(e => new OrderDTO
                {
                    Id = e.Id,
                    CreatedAt = e.CreatedAt,
                    FulfilledAt = e.FulfilledAt,
                    Status = e.Status.Name,
                    Client = new ClientInfoDTO
                    {
                        FirstName = e.Client.FirstName,
                        LastName = e.Client.LastName,
                    },
                    Products = e.ProductOrders.Select(p => new OrderLineItemDTO()
                    {
                        Name = p.Product.Name,
                        Price = p.Product.Price,
                        Amount = p.Amount
                    }).ToList()
                
                }).FirstOrDefaultAsync(e => e.Id == orderId);
            
            return resOrder;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            throw;
        }
    }
}