namespace Mango.Web.Services.IServices
{
    public interface ICouponService
    {
        Task<T> GetCouponAsync<T>(string code, string token = null);
    }
}
