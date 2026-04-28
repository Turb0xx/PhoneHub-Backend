using AutoMapper;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;

namespace PhoneHub.Infrastructure.Mappings
{
    public class SaleProfile : Profile
    {
        public SaleProfile()
        {
            CreateMap<Sale, SaleResponseDto>()
                .ForMember(dest => dest.SaleId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Date,
                    opt => opt.ConvertUsing<DateTimeToStringConverter, DateTime>())
                .ForMember(dest => dest.Brand,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Brand : string.Empty))
                .ForMember(dest => dest.Model,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Model : string.Empty))
                .ForMember(dest => dest.UnitPrice,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0))
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.FirstName + " " + src.User.LastName : string.Empty))
                .ForMember(dest => dest.CustomerEmail,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));
        }
    }
}
