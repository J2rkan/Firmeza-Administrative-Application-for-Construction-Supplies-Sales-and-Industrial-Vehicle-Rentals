using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Core.Entities;

namespace Firmeza.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Client mappings
        CreateMap<Client, ClientDto>();
        CreateMap<CreateClientDto, Client>();
        CreateMap<UpdateClientDto, Client>();

        // Sale mappings
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name));
        
        CreateMap<SaleDetail, SaleDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
        
        CreateMap<CreateSaleDto, Sale>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Total, opt => opt.Ignore());
        
        CreateMap<CreateSaleDetailDto, SaleDetail>()
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore());
    }
}
