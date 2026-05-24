using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;

namespace PhoneHub.Core.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<IEnumerable<UserDto>> GetAllDapperAsync();
        Task<UserDto?> GetByIdDapperAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByCredentialsAsync(string email, string hashedPassword);
    }
}
