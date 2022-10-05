﻿using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Configurations
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDto>().ReverseMap();
                config.CreateMap<Cart, CartDto>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            });

            return mappingConfig;
        }
    }
}
