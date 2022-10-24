using Mongo.Services.OrderAPI.Models;

namespace Mongo.Services.OrderAPI.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrderAsync(OrderHeader orderHeader);
        Task UpdateOrderPaymentStatus(int orderHeaderId, bool isPaid);
    }
}
