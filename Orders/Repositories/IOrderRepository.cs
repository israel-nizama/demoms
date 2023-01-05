using Orders.Entities;

namespace Orders.Repositories
{
    public interface IOrderRepository
    {
        Task AddOrder(Order order);
        Task UpdateOrderPaymentStatus(Guid orderId, bool paid);
        Task<List<Order>> GetOrdersForUser(Guid userId);
    }
}
