using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;

namespace PhoneHub.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<User?> GetByCredentialsAsync(UserLogin userLogin);
        Task CreateUserAsync(CreateUserDto dto);
        Task UpdateUserAsync(int id, CreateUserDto dto);
        Task DeactivateUserAsync(int id);
        Task ActivateUserAsync(int id);
    }
}
