using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        protected ResponceDto _responce;
        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            this._responce = new ResponceDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserIdAsync(userId);               
                _responce.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _responce;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cart)
        {
            try
            {
                CartDto cartDto = await _cartRepository.CreateUpdateCartAsync(cart);
                _responce.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _responce;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cart)
        {
            try
            {
                CartDto cartDto = await _cartRepository.CreateUpdateCartAsync(cart);
                _responce.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _responce;
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody]int cartId)
        {
            try
            {
                bool result = await _cartRepository.RemoveFromCartAsync(cartId);
                _responce.Result = result;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _responce;
        }

        [HttpPost("ClearCart")]
        public async Task<object> ClearCart([FromBody] string userId)
        {
            try
            {
                bool result = await _cartRepository.ClearCartAsync(userId);
                _responce.Result = result;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _responce;
        }
    }
}
