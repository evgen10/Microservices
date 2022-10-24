using AutoMapper;
using Mango.Services.CouponAPI.DbContexts;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public CouponRepository(ApplicationDbContext dbContext, IMapper _mapper)
        {
            _dbContext = dbContext;
            this._mapper = _mapper;
        }

        public async Task<CouponDto> GetCouponByCode(string code)
        {
            var couponFromDb = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.Code == code);
            return _mapper.Map<CouponDto>(couponFromDb);
        }
    }
}
