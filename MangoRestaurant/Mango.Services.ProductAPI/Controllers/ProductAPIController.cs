using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        protected ResponceDto _responce;
        private IProductRepository _repository;

        public ProductAPIController(IProductRepository repository)
        {
            this._responce = new ResponceDto();
            _repository = repository;
        }

        [Authorize]
        [HttpGet]
        public async Task<ResponceDto> Get()
        {
            try
            {
                IEnumerable<ProductDto> productDtos = await _repository.GetProducts();
                _responce.Result = productDtos;                
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message.ToString() };                
            }

            return _responce;
        }

        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<ResponceDto> Get(int id)
        {
            try
            {
                ProductDto productDto = await _repository.GetProductById(id);
                _responce.Result = productDto;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message.ToString() };
            }

            return _responce;
        }

        [Authorize]
        [HttpPost]
        public async Task<ResponceDto> Post([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto model = await _repository.CreateUpdateProduct(productDto);
                _responce.Result = model;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message.ToString() };
            }

            return _responce;
        }

        [Authorize]
        [HttpPut]
        public async Task<ResponceDto> Put([FromBody] ProductDto productDto)
        {
            try
            {
                ProductDto model = await _repository.CreateUpdateProduct(productDto);
                _responce.Result = model;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message.ToString() };
            }

            return _responce;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ResponceDto> Delete(int id)
        {
            try
            {
                bool isSuccess = await _repository.DeleteProduct(id);
                _responce.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message.ToString() };
            }

            return _responce;
        }
    }
}
