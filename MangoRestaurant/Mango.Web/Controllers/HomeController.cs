using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(
            ILogger<HomeController> logger,
            IProductService productService,
            ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> products = new();
            var responce = await _productService.GetAllProductsAsync<ResponceDto>("");

            if (responce != null && responce.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responce.Result));
            }

            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto product = new();
            var responce = await _productService.GetProductByIdAsync<ResponceDto>(productId,"");

            if (responce != null && responce.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responce.Result));
            }

            return View(product);
        }

        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductDto product)
        {
            CartDto cartDto = new()
            {
                CartHeader = new()
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetailsDto = new()
            {
                Count = product.Count,
                ProductId = product.ProductId
            };

            var resp = await _productService.GetProductByIdAsync<ResponceDto>(product.ProductId, "");
            if (resp != null && resp.IsSuccess)
            {
                cartDetailsDto.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result));
            }

            List<CartDetailsDto> cartDetailsList = new();
            cartDetailsList.Add(cartDetailsDto);
            cartDto.CartDetails = cartDetailsList;

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var addCartResponce = await _cartService.AddToCartAsync<ResponceDto>(cartDto, accessToken);
            if (addCartResponce != null && addCartResponce.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            return RedirectToAction(nameof(Index));
        }
                
        public IActionResult Logout()
        {
            return SignOut("Cookies","oidc");
        }
    }
}