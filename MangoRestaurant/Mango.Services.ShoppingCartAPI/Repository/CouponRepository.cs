using Mango.Services.ShoppingCartAPI.Models.Dto;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _httpClient;

        public CouponRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CouponDto> GetCouponAsync(string code)
        {
            var responce = await _httpClient.GetAsync($"/api/coupon/{code}");
            var apiContent = await responce.Content.ReadAsStringAsync();
            var couponResponce = JsonConvert.DeserializeObject<ResponceDto>(apiContent);
            if (couponResponce != null && couponResponce.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(couponResponce.Result));
            }

            return new CouponDto();
        }
    }
}
