using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Configurations;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/Cart")]
    [ApiController]
    public class CartApiController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMessageBus _messageBus;
        private readonly ICouponRepository _couponRepository;
        private readonly ServiceBusConfig _serviceBusOptions;
        protected ResponceDto _responce;

        public CartApiController(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository, IOptions<ServiceBusConfig> options)
        {
            _cartRepository = cartRepository;
            _messageBus = messageBus;
            _serviceBusOptions = options.Value;
            _couponRepository = couponRepository;
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

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cart)
        {
            try
            {
                bool result = await _cartRepository.ApplyCouponAsync(cart.CartHeader.UserId, cart.CartHeader.CouponCode);
                _responce.Result = result;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _responce;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                bool result = await _cartRepository.RemoveCouponAsync(userId);
                _responce.Result = result;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _responce;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeaderDto)
        {
            try
            {
                var cartDto = await _cartRepository.GetCartByUserIdAsync(checkoutHeaderDto.UserId);
                if (cartDto == null)
                {
                    return BadRequest();
                }
                if (!string.IsNullOrEmpty(checkoutHeaderDto.CouponCode))
                {
                    var coupon = await _couponRepository.GetCouponAsync(checkoutHeaderDto.CouponCode);
                    if (coupon.DiscountAmount != checkoutHeaderDto.DiscountTotal)
                    {
                        _responce.IsSuccess = false;
                        _responce.ErrorMessages = new List<string> { "Coupon price has changed, please confirm!" };
                        _responce.DisplayMessage = "Coupon price has changed, please confirm!";
                        return _responce;
                    }
                }


                checkoutHeaderDto.CartDetails = cartDto.CartDetails;
                await _messageBus.PublishMessageAsync(checkoutHeaderDto, _serviceBusOptions.CheckoutQueueName);
                await _cartRepository.ClearCartAsync(checkoutHeaderDto.UserId);
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
