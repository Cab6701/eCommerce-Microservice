using eCommerce.SharedLibrary.Interface;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;

namespace OrderApi.Application.Interfaces
{
    public interface IOrder : IGenericInterface<Order>
    {
        Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate);
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> GetOrderByProductIdAsync(int productId);
        Task<Order> GetOrderByClientIdAsync(int clientId);
        Task<Order> GetOrderByOrderedDateAsync(DateTime orderedDate);
        Task<Order> GetOrderByOrderedDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}