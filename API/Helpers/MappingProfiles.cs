using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;

namespace API.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Product, ProductToReturnDto>()
            .ForMember(
                productDto => productDto.ProductBrandName,
                configurationExpression => configurationExpression.MapFrom(
                    product => product.ProductBrand.Name))
            .ForMember(
                productDto => productDto.ProductTypeName,
                configurationExpression => configurationExpression.MapFrom(
                    product => product.ProductType.Name))
            .ForMember(
                productDto => productDto.PictureUrl,
                configurationExpression => configurationExpression.MapFrom<ProductUrlResolver>());

        CreateMap<Address, AddressDto>().ReverseMap();
        CreateMap<CustomerBasketDto, CustomerBasket>();
        CreateMap<BasketItemDto, BasketItem>();
        CreateMap<AddressDto, Core.Entities.OrderAggregate.Address>();
        CreateMap<Core.Entities.OrderAggregate.Order, OrderToReturnDto>()
            .ForMember(
                orderToReturnDto => orderToReturnDto.DeliveryMethod,
                configurationExpression => configurationExpression.MapFrom(order => order.DeliveryMethod.ShortName))
            .ForMember(
                orderToReturnDto => orderToReturnDto.ShippingPrice,
                configurationExpression => configurationExpression.MapFrom(order => order.DeliveryMethod.Price));
        CreateMap<Core.Entities.OrderAggregate.OrderItem, OrderItemDto>()
            .ForMember(
                orderToReturnDto => orderToReturnDto.ProudctId,
                configurationExpression => configurationExpression.MapFrom(orderItem => orderItem.ItemOrdered.ProductItemId))
            .ForMember(
                orderToReturnDto => orderToReturnDto.ProductName,
                configurationExpression => configurationExpression.MapFrom(orderItem => orderItem.ItemOrdered.ProductName))
            .ForMember(
                orderToReturnDto => orderToReturnDto.PictureUrl,
                configurationExpression => configurationExpression.MapFrom(orderItem => orderItem.ItemOrdered.PictureUrl))
            .ForMember(
                orderToReturnDto => orderToReturnDto.PictureUrl,
                configurationExpression => configurationExpression.MapFrom<OrderItemUrlResolver>());
    }
}
