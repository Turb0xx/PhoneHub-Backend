using PhoneHub.Core.Entities;
using System.Data;

namespace PhoneHub.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository ProductRepository { get; }
        ISaleRepository SaleRepository { get; }
        IBaseRepository<User> UserRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        IDbConnection? GetDbConnection();
        IDbTransaction? GetDbTransaction();
    }
}
