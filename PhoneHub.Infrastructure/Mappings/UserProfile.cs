using AutoMapper;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;

namespace PhoneHub.Infrastructure.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
