using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrder
{
    public async Task<Response> CreateAsync(Order entity)
    {
        try
        {
            var order = context.Orders.Add(entity).Entity;
            await context.SaveChangesAsync();
            return order.Id > 0 ? new Response(true, "Order placed successfully") : new Response(false, "Error occured while placing order");
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            return new Response(false, "Error occured while placing order");
        }
    }

    public async Task<Response> DeleteAsync(Order entity)
    {
        try
        {
            var order = await FindByIdAsync(entity.Id);
            if (order is null)
                return new Response(false, "Order does not exist");
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            return new Response(true, "Order deleted successfully");
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            return new Response(false, "Error occured while deleting order");
        }
    }

    public async Task<Order> FindByIdAsync(int id)
    {
        try
        {
            var order = await context.Orders.FindAsync(id);
            return order!;

        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            throw new Exception("Error occured while retrieving order");
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        try
        {
            var orders = await context.Orders.AsNoTracking().ToListAsync();
            return orders!;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            throw new Exception("Error occured while retrieving order");
        }
    }

    public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var order = await context.Orders.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
            return order!;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            throw new Exception("Error occured while retrieving order");
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var orders = await context.Orders.AsNoTracking().Where(predicate).ToListAsync();
            return orders!;
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            throw new Exception("Error occured while placing order");
        }
    }

    public async Task<Response> UpdateAsync(Order entity)
    {
        try
        {
            var order = await FindByIdAsync(entity.Id);
            if (order is null)
                return new Response(false, "Order does not exist");
            context.Entry(order).State = EntityState.Detached;
            context.Orders.Update(entity);
            await context.SaveChangesAsync();
            return new Response(true, "Order updated successfully");
        }
        catch (Exception ex)
        {
            LogException.LogExceptions(ex);
            return new Response(false, "Error occured while placing order");
        }
    }
}
