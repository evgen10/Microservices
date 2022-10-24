using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;

        public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartByUser());
        }

        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartByUser());
        }

        public async Task<IActionResult> Confirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(CartDto cartDto)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var responce = await _cartService.CheckoutAsync<ResponceDto>(cartDto.CartHeader, accessToken);
            if (!responce.IsSuccess)
            {
                TempData["Error"] = responce.DisplayMessage;
                return RedirectToAction(nameof(CheckOut)); 

            }


            return RedirectToAction(nameof(Confirmation));
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var responce = await _cartService.RemoveFromCartAsync<ResponceDto>(cartDetailsId, accessToken);
            if (responce != null && responce.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCouponAsync<ResponceDto>(cartDto.CartHeader.UserId, accessToken);


            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplayCouponAsync<ResponceDto>(cartDto, accessToken);


            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }


        private async Task<CartDto> LoadCartByUser()
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userCartResponce = await _cartService.GetCartByUserIdAsync<ResponceDto>(userId, accessToken);

            CartDto cartDto = new();
            if (userCartResponce != null && userCartResponce.IsSuccess)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(userCartResponce.Result));
            }

            if (cartDto.CartHeader != null)
            {
                var couponResopnce = await _couponService.GetCouponAsync<ResponceDto>(cartDto.CartHeader.CouponCode, accessToken);

                if (couponResopnce != null && couponResopnce.IsSuccess)
                {
                    var couponObj = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(couponResopnce.Result));
                    if (couponObj != null)
                    {
                        cartDto.CartHeader.DiscountTotal = couponObj.DiscountAmount;
                    }
                }

                foreach (var detail in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotal += (detail.Product.Price * detail.Count);                    
                }

                cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
            }

            return cartDto;
        }
    }
}
