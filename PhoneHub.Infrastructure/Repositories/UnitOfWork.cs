using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PhoneHub.Core.Entities;
using PhoneHub.Core.Interfaces;
using PhoneHub.Infrastructure.Data;
using System.Data;

namespace PhoneHub.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PhoneHubContext _context;
        private readonly IDapperContext _dapper;

        private IProductRepository? _productRepository;
        private ISaleRepository? _saleRepository;
        private IBaseRepository<User>? _userRepository;

        private IDbContextTransaction? _efTransaction;

        public UnitOfWork(PhoneHubContext context, IDapperContext dapper)
        {
            _context = context;
            _dapper = dapper;
        }

        public IProductRepository ProductRepository =>
            _productRepository ??= new ProductRepository(_context, _dapper);

        public ISaleRepository SaleRepository =>
            _saleRepository ??= new SaleRepository(_context, _dapper);

        public IBaseRepository<User> UserRepository =>
            _userRepository ??= new BaseRepository<User>(_context);

        public async Task BeginTransactionAsync()
        {
            if (_efTransaction == null)
            {
                _efTransaction = await _context.Database.BeginTransactionAsync();

                var conn = _context.Database.GetDbConnection();
                var tx = _efTransaction.GetDbTransaction();
                _dapper.SetAmbientConnection(conn, tx);
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_efTransaction != null)
                {
                    await _efTransaction.CommitAsync();
                    _efTransaction.Dispose();
                    _efTransaction = null;
                }
            }
            finally
            {
                _dapper.ClearAmbientConnection();
            }
        }

        public async Task RollbackAsync()
        {
            if (_efTransaction != null)
            {
                await _efTransaction.RollbackAsync();
                _efTransaction.Dispose();
                _efTransaction = null;
            }
            _dapper.ClearAmbientConnection();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public IDbConnection? GetDbConnection()
        {
            return _context.Database.GetDbConnection();
        }

        public IDbTransaction? GetDbTransaction()
        {
            return _efTransaction?.GetDbTransaction();
        }

        public void Dispose()
        {
            _efTransaction?.Dispose();
            _context.Dispose();
        }
    }
}
