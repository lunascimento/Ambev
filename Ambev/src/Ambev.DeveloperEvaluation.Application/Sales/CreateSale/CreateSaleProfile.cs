using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleProfile : Profile
    {
        public CreateSaleProfile()
        {
            CreateMap<CreateSaleCommand, Sale>()
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<CreateSaleItemCommand, SaleItem>()
                .ForMember(dest => dest.Total, opt => opt.Ignore());

            CreateMap<Sale, CreateSaleResult>();
            CreateMap<SaleItem, SaleItemResultForCreateSale>();
        }
    }
}
