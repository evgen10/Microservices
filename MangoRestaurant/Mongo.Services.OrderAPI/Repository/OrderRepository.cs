using Microsoft.EntityFrameworkCore;
using Mongo.Services.OrderAPI.DbContexts;
using Mongo.Services.OrderAPI.Models;

namespace Mongo.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddOrderAsync(OrderHeader orderHeader)
        {
            try
            {
                await using var db = new ApplicationDbContext(_dbContext);
                db.OrderHeaders.Add(orderHeader);
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool isPaid)
        {
            await using var db = new ApplicationDbContext(_dbContext);
            var orderHeader = await db.OrderHeaders.FirstOrDefaultAsync(x => x.OrderHeaderId == orderHeaderId);
            if (orderHeader != null)
            {
                orderHeader.PaymentStatus = isPaid;
                await db.SaveChangesAsync();
            }
        }
    }
}
