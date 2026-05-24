using Microsoft.EntityFrameworkCore;
using PhoneHub.Core.DTOs;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Enum;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;
using PhoneHub.Infrastructure.Queries;

namespace PhoneHub.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly IDapperContext _dapper;

        public UserRepository(PhoneHubContext context, IDapperContext dapper) : base(context)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllDapperAsync()
        {
            var sql = _dapper.Provider switch
            {
                DataBaseProvider.SqlServer => UserQueries.GetAllSqlServer,
                DataBaseProvider.MySql => UserQueries.GetAllMySql,
                _ => throw new NotSupportedException("Provider no soportado")
            };
            return await _dapper.QueryAsync<UserDto>(sql);
        }

        public async Task<UserDto?> GetByIdDapperAsync(int id)
        {
            var sql = _dapper.Provider switch
            {
                DataBaseProvider.SqlServer => UserQueries.GetByIdSqlServer,
                DataBaseProvider.MySql => UserQueries.GetByIdMySql,
                _ => throw new NotSupportedException("Provider no soportado")
            };
            return await _dapper.QueryFirstOrDefaultAsync<UserDto>(sql, new { Id = id });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _entities.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByCredentialsAsync(string email, string hashedPassword)
        {
            return await _entities.FirstOrDefaultAsync(
                u => u.Email == email && u.Password == hashedPassword);
        }
    }
}
