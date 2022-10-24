using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserIdAsync(string userId);
        Task<CartDto> CreateUpdateCartAsync(CartDto cartDto);
        Task<bool> RemoveFromCartAsync(int cartDetailsId);
        Task<bool> ClearCartAsync(string userId);
        Task<bool> ApplyCouponAsync(string userId, string code);
        Task<bool> RemoveCouponAsync(string userId);
    }
}
