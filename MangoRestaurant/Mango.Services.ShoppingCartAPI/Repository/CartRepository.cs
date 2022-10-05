using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CartRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _dbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cartHeaderFromDb = await _dbContext.CartHeader
                .FirstOrDefaultAsync(ch => ch.UserId == userId);

            if (cartHeaderFromDb != null)
            {
                _dbContext.CartDetails
                    .RemoveRange(_dbContext.CartDetails
                               .Where(cd => cd.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                _dbContext.CartHeader.Remove(cartHeaderFromDb);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;            
        }

        public async Task<CartDto> CreateUpdateCartAsync(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);
            var prodInDb = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);

            if (prodInDb == null)
            {
                _dbContext.Add(cart.CartDetails.FirstOrDefault().Product);
                await _dbContext.SaveChangesAsync();        
            }

            var cartHeaderFromDb = await _dbContext.CartHeader
                .AsNoTracking()
                .FirstOrDefaultAsync(ch => ch.UserId == cart.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                _dbContext.CartHeader.Add(cart.CartHeader);
                await _dbContext.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb = await _dbContext.CartDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        cd => cd.ProductId == cart.CartDetails.FirstOrDefault().ProductId
                     && cd.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (cartDetailsFromDb == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _dbContext.Add(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var currentCurt = cart.CartDetails.FirstOrDefault();

                    currentCurt.Count += cartDetailsFromDb.Count;
                    currentCurt.CartHeaderId = cartDetailsFromDb.CartHeaderId;
                    currentCurt.CartDetailsId = cartDetailsFromDb.CartDetailsId;

                    cart.CartDetails.FirstOrDefault().Product = null;
                    _dbContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserIdAsync(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _dbContext.CartHeader.FirstOrDefaultAsync(h => h.UserId == userId),

            };
            cart.CartDetails = _dbContext.CartDetails
                .Where(cd => cd.CartHeaderId == cart.CartHeader.CartHeaderId).Include(x => x.Product);

            return _mapper.Map<CartDto>(cart); 
        }

        public async Task<bool> RemoveFromCartAsync(int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _dbContext.CartDetails
                    .FirstOrDefaultAsync(x => x.CartDetailsId == cartDetailsId);

                int totalCounOfCartItems = _dbContext.CartDetails
                    .Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();

                _dbContext.Remove(cartDetails);

                if (totalCounOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _dbContext.CartHeader
                        .FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);
                    _dbContext.Remove(cartHeaderToRemove);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
