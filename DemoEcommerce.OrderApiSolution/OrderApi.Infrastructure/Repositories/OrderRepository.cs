using System.Linq.Expressions;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;

namespace OrderApi.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrder
{
    public async Task<Response> CreateAsync(Order entity)
    {
        try
        {
            var currentEntity = context.Orders.Add(entity).Entity;
            await context.SaveChangesAsync();
            if (currentEntity != null && currentEntity.Id > 0)
            {
                return new Response(true, $"{entity.Id} added to database successfully");
            }
            else
            {
                return new Response(false, $"Error occurred while adding {entity.Id}");
            }
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            return new Response(false, "Error occurred while adding order");
        }
    }

    public async Task<Response> DeleteAsync(Order entity)
    {
        try
        {
            var order = await FindByIdAsync(entity.Id);
            if (order is null)
                return new Response(false, $"{entity.Id} not found");

            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            return new Response(true, $"{entity.Id} deleted from database successfully");
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            return new Response(false, "Error occurred while deleting order");
        }
    }

    public async Task<Order> FindByIdAsync(int id)
    {
        try
        {
            return await context.Orders.FindAsync(id) ?? null!;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        try
        {
            return await context.Orders.AsNoTracking().ToListAsync() ?? null!;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            throw new Exception(ex.Message);
        }
    }

    public async Task<Order> GetByAsync(Expression<Func<Order, bool>> expression)
    {
        try
        {
            return await context.Orders.Where(expression).FirstOrDefaultAsync() ?? null!;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            return await context.Orders.Where(predicate).ToListAsync() ?? null!;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            throw new Exception(ex.Message);
        }
    }

    public async Task<Response> UpdateAsync(Order entity)
    {
        try
        {
            var order = await FindByIdAsync(entity.Id);
            if (order is null)
                return new Response(false, $"{entity.Id} not found");

            context.Entry(order).State = EntityState.Detached;
            context.Orders.Update(entity);
            await context.SaveChangesAsync();
            return new Response(true, $"{entity.Id} updated in database successfully");
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            return new Response(false, "Error occurred while updating order");
        }
    }
}