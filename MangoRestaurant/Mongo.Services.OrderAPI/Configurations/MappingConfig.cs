using AutoMapper;
using Mongo.Services.OrderAPI.Messages;
using Mongo.Services.OrderAPI.Models;

namespace Mongo.Services.OrderAPI.Configurations
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CheckoutHeaderDto, OrderHeader>()
                    .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(opt => new List<OrderDetails>()))
                    .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(opt => false))
                    .ForMember(dest => dest.OrderTime, opt => opt.MapFrom(opt => DateTime.Now));

                config.CreateMap<CartDetailsDto, OrderDetails>()
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(opt => opt.Product.Name))
                    .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(opt => opt.Product.Price));

            });

            return mappingConfig;
        }
    }
}
